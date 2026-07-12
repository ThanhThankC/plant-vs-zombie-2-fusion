using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CircleButton))]
[RequireComponent(typeof(Image))]
public class GloveButton : MonoBehaviour, IDraggableButton, ISelectableButton
{
    [SerializeField] private Sprite normalGloveSprite;
    [SerializeField] private Sprite pressedGloveSprite;

    private ToolManager toolManager;
    private DragController dragController;
    private Image buttonImage;

    private void Start()
    {
        toolManager = ToolManager.Instance;
        dragController = DragController.Instance;
        buttonImage = GetComponent<Image>();
        dragController.OnDragEnd += Deselect;
    }

    private void OnDestroy()
    {
        if (dragController != null) dragController.OnDragEnd -= Deselect;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        dragController.RefreshTool();
        dragController.BeginDrag(ToolType.Glove);
        toolManager.Glove?.SetActive(false);
        Select();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragController.RefreshTool();
        dragController.BeginDrag(ToolType.Glove);
        toolManager.Glove?.SetActive(true);
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
}
