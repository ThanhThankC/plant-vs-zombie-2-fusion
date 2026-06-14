using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasInputReceiver : Singleton<CanvasInputReceiver>, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private LayerMask zoneLayer;

    private Camera mainCamera;
    private DragController dragController;
    private Zone zoneHover;
    private bool isHovering;

    private void Start()
    {
        mainCamera = Camera.main;
        dragController = DragController.Instance;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        dragController.ShowToolAt(GetMouseWorldPos(eventData));
        dragController.FlashThenEndDrag();
        dragController.NotifyBeginDrag();
        OnEndDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragController.IsDragging) return;

        dragController.ShowToolAt(GetMouseWorldPos(eventData));
        dragController.NotifyBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragController.IsDragging) return;
        dragController.ShowToolAt(GetMouseWorldPos(eventData));
        HandleHover(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragController.IsDragging) return;
        Collider2D hit = Physics2D.OverlapPoint(GetMouseWorldPos(eventData), zoneLayer);
        Zone zone = hit?.GetComponent<Zone>();

        zone?.OnZoneInteract();
        CancelHover();
        dragController.TryEndDrag();
    }

    private void HandleHover(PointerEventData eventData)
    {
        Collider2D hit = Physics2D.OverlapPoint(GetMouseWorldPos(eventData), zoneLayer);
        Zone zone = hit?.GetComponent<Zone>();

        if (zoneHover != null && (zone != null && zone.Cell != zoneHover.Cell || zone == null))
        {
            if (isHovering)
            {
                isHovering = false;
                zoneHover.OnDragHoverExit();
                zoneHover = null;
            }
        }
        else if (zone != null && dragController.CanHover(zone))
        {
            if (!isHovering)
            {
                isHovering = true;
                zone.OnDragHoverEnter();
                zoneHover = zone;
            }
        }
    }

    private void CancelHover()
    {
        if (!isHovering || zoneHover == null) return;
        isHovering = false;
        zoneHover.OnDragHoverExit();
        zoneHover = null;
    }


    private Vector3 GetMouseWorldPos(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));
    }
}
