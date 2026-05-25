using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : Singleton<DragHandler>, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI DebugText;
    [SerializeField] private LayerMask interactiveLayer;

    private Camera mainCamera;
    private PlantManager plantManager;
    private Zone zoneHover;
    private bool isHovering;

    private void Start()
    {
        mainCamera = Camera.main;
        plantManager = PlantManager.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!plantManager.IsDragging) return;

        plantManager.ToggleGhostPlantVisual(true);
        if (plantManager.GhostPlant != null)
            plantManager.GhostPlant.transform.position = GetMouseWorldPos(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!plantManager.IsDragging) return;
        if (plantManager.GhostPlant != null)
            plantManager.GhostPlant.transform.position = GetMouseWorldPos(eventData);

        HandleHover(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!plantManager.IsDragging) return;
        Collider2D hit = Physics2D.OverlapPoint(GetMouseWorldPos(eventData), interactiveLayer);
        Zone zone = hit?.GetComponent<Zone>();

        DebugText.text = "EndDrag --- zone: " + zone;
        zone?.OnZoneInteract();
        plantManager.EndDrag();
    }

    public void OnPointerClick(PointerEventData eventData) => OnEndDrag(eventData);

    private void HandleHover(PointerEventData eventData)
    {
        Collider2D hit = Physics2D.OverlapPoint(GetMouseWorldPos(eventData), interactiveLayer);
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
        else if (zone != null)
        {
            if (!isHovering)
            {
                isHovering = true;
                zone.OnDragHoverEnter();
                zoneHover = zone;
            }
        }
    }

    private Vector3 GetMouseWorldPos(PointerEventData eventData)
    {
        Vector2 screenPos = eventData.position;
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));
    }
}
