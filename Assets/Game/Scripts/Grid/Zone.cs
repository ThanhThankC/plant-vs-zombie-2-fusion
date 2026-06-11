using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldType { Normal, Support }

public class Zone : MonoBehaviour
{
    [SerializeField] private FieldType fieldType;

    public FieldType FieldType => fieldType;
    public Cell Cell => cell;

    private Cell cell;
    private DragController dragController;
    private PlantManager plantManager;
    private ToolManager toolManager;

    private void Awake()
    {
        cell = GetComponentInParent<Cell>();
        if (cell == null)
            Debug.LogWarning($"[Zone] Not found Cell in parent!");
    }

    private void Start()
    {
        dragController = DragController.Instance;
        plantManager = PlantManager.Instance;
        toolManager = ToolManager.Instance;
    }

    public void OnZoneInteract()
    {
        if (plantManager.IsDraggingFromCell(cell)) return;
        if (!dragController.IsDragging) return;
        if (dragController.CurrentToolType == ToolType.Glove)
        {
            if (!toolManager.MovingPlant)
            {
                var instance = GetPlantInCell();
                if (instance == null) return;
                toolManager.SelectPlantWithGlove(instance, cell);
            }
        }
        else if (dragController.CurrentToolType == ToolType.None )
        {
            var result = GetFusionResult();
            bool canPlace = result.HasValue;

            if (!canPlace) return;

            plantManager.PlacePlant(cell, result.Value);
            if (toolManager.MovingPlant) toolManager.EndDrag();
        }
        else if (dragController.CurrentToolType == ToolType.Shovel)
        {
            var instance = GetPlantInCell();
            if (instance == null) return;
            toolManager.RemovePlantWithShovel(instance, cell);
        }
    }

    public void OnDragHoverEnter()
    {
        if (plantManager.IsDraggingFromCell(cell)) return;
        if (!dragController.IsDragging) return;

        if (dragController.CurrentToolType == ToolType.Shovel || dragController.CurrentToolType == ToolType.Glove)
        {
            cell.ToggleHighlight(true); return;
        }
        bool canPlace = GetFusionResult().HasValue;
        cell.ToggleHighlight(canPlace);
    }

    public void OnDragHoverExit()
    {
        if (!dragController.IsDragging) return;
        cell.ToggleHighlight(false);
    }

    private PlantBase GetPlantInCell() => cell.GetPlantInstance(fieldType);

    private PlantType? GetFusionResult()
    {
        if (!plantManager.CurrentPlantType.HasValue) return null;
        return GetFusionResult(plantManager.CurrentPlantType.Value);
    }

    public PlantType? GetFusionResult(PlantType incoming)
    {
        var primaryExisting = fieldType == FieldType.Normal ? cell.NormalPlant : cell.SupportPlant;
        var otherExisting = fieldType == FieldType.Normal ? cell.SupportPlant : cell.NormalPlant;
        return FusionDatabaseMono.DB.GetPlantResult(incoming, primaryExisting, otherExisting);
    }
}
