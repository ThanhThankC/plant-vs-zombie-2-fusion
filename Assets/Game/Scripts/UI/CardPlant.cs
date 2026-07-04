using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPlant : MonoBehaviour, IDraggableButton, ISelectableButton
{
    //[SerializeField] private PlantData data;
    [SerializeField] private PlantType plantType;
    [SerializeField] private Image cardImage;
    [SerializeField] private Image selectedFrame;

    //public PlantType PlantType => data.plantType;

    private PlantManager plantManager;
    private DragController dragController;

    private void Start()
    {
        plantManager = PlantManager.Instance;
        dragController = DragController.Instance;

        var data = plantManager.GetPlantData(plantType);
        if (cardImage != null && data != null)
            cardImage.sprite = data.cardSprite;

        dragController.OnDragEnd += Deselect;
    }

    private void OnDestroy()
    {
        dragController.OnDragEnd -= Deselect;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        dragController.RefreshTool();
        plantManager.OnCardClicked(plantType);
        dragController.BeginDrag(ToolType.None);
        Select();
        plantManager.ToggleGhostPlantVisual(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragController.RefreshTool();
        plantManager.OnCardClicked(plantType);
        dragController.BeginDrag(ToolType.None);
        Select();
        plantManager.ToggleGhostPlantVisual(true);
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

    public void Select() => selectedFrame?.gameObject.SetActive(true);

    public void Deselect() => selectedFrame?.gameObject.SetActive(false);
} 
