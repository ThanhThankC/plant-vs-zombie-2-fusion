using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CircleButton))]
[RequireComponent(typeof(Image))]
public class GloveButton : MonoBehaviour, IDraggableButton, ISelectableButton
{
    [Header("Events")]
    [SerializeField] private OnPlantPlacedEvent OnPlantPlaced;

    [SerializeField] private Sprite normalGloveSprite;
    [SerializeField] private Sprite pressedGloveSprite;
    [SerializeField] protected Image cooldownMask;
    [SerializeField] protected float cooldownDuration = 1.5f;

    private ToolManager toolManager;
    private DragController dragController;
    private Image buttonImage;
    private CircleButton circleButton;
    private bool isOnCooldown = false;

    private void Start()
    {
        toolManager = ToolManager.Instance;
        dragController = DragController.Instance;
        buttonImage = GetComponent<Image>();
        circleButton = GetComponent<CircleButton>();

        dragController.OnDragEnd += Deselect;
        if (OnPlantPlaced != null) OnPlantPlaced.OnRaised += TriggerCooldown;
        cooldownMask.fillAmount = 0f;
    }

    private void OnDestroy()
    {
        if (dragController != null) dragController.OnDragEnd -= Deselect;
        if (OnPlantPlaced != null) OnPlantPlaced.OnRaised -= TriggerCooldown;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOnCooldown) return;
        dragController.RefreshTool();
        dragController.BeginDrag(ToolType.Glove);
        toolManager.Glove?.gameObject.SetActive(false);
        Select();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isOnCooldown) return;
        dragController.RefreshTool();
        dragController.BeginDrag(ToolType.Glove);
        toolManager.Glove?.gameObject.SetActive(true);
        Select();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragController.IsDragging) return;
        CanvasInputReceiver.Instance.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragController.IsDragging) return;
        CanvasInputReceiver.Instance.OnEndDrag(eventData);
    }

    public void Select() => buttonImage.sprite = pressedGloveSprite;
    public void Deselect() => buttonImage.sprite = normalGloveSprite;

    private void TriggerCooldown()
    {
        if (isOnCooldown) return;
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        Deselect();
        if (circleButton != null) circleButton.enabled = false;

        cooldownMask.gameObject.SetActive(true);
        cooldownMask.fillAmount = 1f;

        float elapsed = 0f;
        while (elapsed < cooldownDuration)
        {
            elapsed += Time.deltaTime;
            cooldownMask.fillAmount = 1f - (elapsed / cooldownDuration);
            yield return null;
        }

        cooldownMask.fillAmount = 0f;
        cooldownMask.gameObject.SetActive(false);
        if (circleButton != null) circleButton.enabled = true;
        isOnCooldown = false;
    }
}