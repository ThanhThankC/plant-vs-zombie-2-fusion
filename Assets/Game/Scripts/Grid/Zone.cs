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
    private PlantManager plantManager;

    private void Awake()
    {
        cell = GetComponentInParent<Cell>();
        if (cell == null)
            Debug.LogWarning($"[Zone] Not found Cell in parent!");
    }

    private void Start()
    {
        plantManager = PlantManager.Instance;
        if (plantManager == null)
            Debug.LogWarning($"[Zone] Not found PlantManager!");
    }

    public void OnZoneInteract()
    {
        if (plantManager.CurrentTool == ToolType.Glove)
        {
            var instance = cell.GetPlantInstance(fieldType);
            if (instance == null)
            {
                var otherField = fieldType == FieldType.Normal ? FieldType.Support : FieldType.Normal;
                instance = cell.GetPlantInstance(otherField);
            }

            if (instance == null) return;

            plantManager.OnCellClicked(instance, cell, fieldType);
        }
        else if (plantManager.CurrentTool == ToolType.None)
        {
            cell.ToggleHighlight(false);
            var result = GetFusionResult();
            bool canPlace = result.HasValue;

            if (!canPlace) return;

            var targetFieldType = result.Value.GetFieldType();
            plantManager.PlacePlant(cell, targetFieldType, result.Value);
        }
    }

    public void OnDragHoverEnter()
    {
        if (PlantManager.Instance.IsDraggingFromCell(cell)) return;
        if (!PlantManager.Instance.IsDragging) return;

        bool canPlace = GetFusionResult().HasValue;
        cell.ToggleHighlight(canPlace);
    }

    public void OnDragHoverExit()
    {
        if (!PlantManager.Instance.IsDragging) return;
        cell.ToggleHighlight(false);
    }

    private PlantType? GetFusionResult()
    {
        if (!plantManager.CurrentPlantType.HasValue) return null;

        var incoming = plantManager.CurrentPlantType.Value;
        var primaryExisting = fieldType == FieldType.Normal ? cell.NormalPlant : cell.SupportPlant;
        var otherExisting = fieldType == FieldType.Normal ? cell.SupportPlant : cell.NormalPlant;
        return FusionDatabaseMono.DB.GetPlantResult(incoming, primaryExisting, otherExisting);
    }
}
