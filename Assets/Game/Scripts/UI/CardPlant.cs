using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPlant : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private PlantData data;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image seletedFrame;

    public PlantType PlantType => data.plantType;

    private PlantManager plantManager;

    private void Start()
    {
        plantManager = PlantManager.Instance;
        if (plantManager == null)
            Debug.LogWarning($"[Zone] Not found PlantManager!");

        if (cardImage != null)
            cardImage.sprite = data.cardSprite;

        plantManager.OnDragEnd += DeselectFrameCard;
    }

    private void OnDestroy()
    {
        plantManager.OnDragEnd -= DeselectFrameCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (plantManager.CurrentTool != ToolType.None) return;

        plantManager.EndDrag();
        SelectedFrameCard();
        plantManager.OnCardClicked(PlantType);
        plantManager.ToggleGhostPlantVisual(false);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (plantManager.CurrentTool != ToolType.None) return;

        plantManager.EndDrag();
        SelectedFrameCard();
        plantManager.OnCardClicked(PlantType);
        plantManager.ToggleGhostPlantVisual(true);
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if (!plantManager.IsDragging) return;
        DragHandler.Instance.OnDrag(eventData); 
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if (!plantManager.IsDragging) return;
        DragHandler.Instance.OnEndDrag(eventData);
    }

    private void SelectedFrameCard() => seletedFrame?.gameObject.SetActive(true);

    private void DeselectFrameCard() => seletedFrame?.gameObject.SetActive(false);
} 
