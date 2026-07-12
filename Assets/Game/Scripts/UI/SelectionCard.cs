using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionCard : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image mask;

    [Header("Config animation")]
    [SerializeField] private float MoveDuration = 0.35f;
    [SerializeField] private Ease MoveEase = Ease.OutCubic;

    private Canvas rootCanvas;
    private DeckCardSlot deckBar;
    private Transform slotWrapper;
    private bool isGhostCard;
    private bool isAnimating;
    private bool isReturned = true;
    private Transform occupiedSlot;
    private PoolKey plantKey;
    private PlantData plantData;

    public void Init(Canvas root, DeckCardSlot deck, Transform slot, PlantData data, PoolKey key)
    {
        rootCanvas = root;
        deckBar = deck;
        slotWrapper = slot;
        plantKey = key;
        plantData = data;
        Init(false);
    }

    public void Init(bool isGhost)
    {
        isGhostCard = isGhost;
        mask.gameObject.SetActive(isGhost);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isGhostCard) return;
        if (isAnimating) return;

        if (isReturned)
            AnimateToDeckSlot();
        else
            AnimateBackToWrapper();
    }

    public void AnimateToDeckSlot()
    {
        Transform targetSlot = deckBar.ClaimFirstEmptySlot();
        if (targetSlot == null) return;

        isAnimating = true;
        isReturned = false;
        occupiedSlot = targetSlot;
        AudioManager.Instance.PlayCardSelect();
        PlantPreviewManager.Instance.Show(plantKey, plantData);

        transform.SetParent(rootCanvas.transform, worldPositionStays: true);
        transform.DOMove(targetSlot.position, MoveDuration)
            .SetEase(MoveEase)
            .OnComplete(() =>
            {
                transform.SetParent(occupiedSlot, worldPositionStays: false);
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                isAnimating = false;
            });
    }

    public void AnimateBackToWrapper()
    {
        isAnimating = true;
        isReturned = true;
        AudioManager.Instance.PlayCardDeselect();
        PlantPreviewManager.Instance.Show(plantKey, plantData);

        if (occupiedSlot != null)
        {
            deckBar.ReleaseSlot(occupiedSlot);
            occupiedSlot = null;
        }

        transform.SetParent(rootCanvas.transform, worldPositionStays: true);
        transform.DOMove(slotWrapper.position, MoveDuration)
            .SetEase(MoveEase)
            .OnComplete(() =>
            {
                transform.SetParent(slotWrapper, worldPositionStays: false);
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.SetAsLastSibling();
                deckBar.CompactSlots();
                isAnimating = false;
            });
    }
}