using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlantManager : Singleton<PlantManager>
{
    [System.Serializable]
    private struct PlantEntry
    {
        public PlantData data;
        public PlantBase prefab;
    }

    [SerializeField] private List<PlantEntry> plantEntries;

    public event Action OnDragEnd;

    public bool IsDragging { get; private set; } = false;
    public PlantBase GhostPlant { get; private set; }
    public PlantType? CurrentPlantType => dragContext?.PlantType;
    public ToolType CurrentTool => ToolType.None;

    private Dictionary<PlantType, PlantBase> prefabLookup = new();
    private Dictionary<PlantType, PlantData> dataLookup = new();
    private DragContext dragContext;


    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in plantEntries)
        {
            prefabLookup[entry.data.plantType] = entry.prefab;
            dataLookup[entry.data.plantType] = entry.data;
        }
    }

    public void OnCardClicked(PlantType plantType)
    {
        if (IsDragging) return;
        IsDragging = true;

        dragContext = new DragContext
        {
            DragSource = DragSource.Deck,
            PlantType = plantType
        };
        SpawnGhost(plantType);
    }

    public void OnCellClicked(PlantBase plant, Cell cell, FieldType fieldType)
    {
        if (IsDragging) return;
        IsDragging = true;

        dragContext = new DragContext
        {
            Plant = plant,
            DragSource = DragSource.Cell,
            SourceCell = cell,
            SourceFieldType = fieldType,
            PlantType = plant.PlantType
        };
        SpawnGhost(plant.PlantType);
    }

    public void PlacePlant(Cell cell, FieldType fieldType, PlantType result)
    {
        DragContext ctx = dragContext;
        bool isFusion = ctx.PlantType != result;

        if (ctx.DragSource == DragSource.Cell)
        {
            if (isFusion) Destroy(ctx.Plant.gameObject);
            ctx.SourceCell.ClearPlant(ctx.SourceFieldType);
        }

        if (isFusion) DestroyPlantAt(cell, fieldType);

        PlantBase plantToPlace;
        if (!isFusion && ctx.DragSource == DragSource.Cell)
        {
            plantToPlace = ctx.Plant;
        }
        else
        {
            plantToPlace = Instantiate(prefabLookup[result]);
            plantToPlace.Init(dataLookup[result]);
        }

        plantToPlace.transform.position = cell.transform.position;
        plantToPlace.SetupAsReal(cell, fieldType);
        cell.SetPlant(fieldType, result, plantToPlace);

        EndDrag();
    }

    public void EndDrag()
    {
        IsDragging = false;
        if (GhostPlant != null) { Destroy(GhostPlant.gameObject); GhostPlant = null; }
        dragContext = null;
        OnDragEnd?.Invoke();
    }

    public bool IsDraggingFromCell(Cell cell) => dragContext?.DragSource == DragSource.Cell && dragContext?.SourceCell == cell;

    private void SpawnGhost(PlantType plantType)
    {
        if (!prefabLookup.TryGetValue(plantType, out var prefab)) return;
        GhostPlant = Instantiate(prefab);
        GhostPlant.transform.SetParent(transform);
        GhostPlant.SetupAsGhost();
    }

    private void DestroyPlantAt(Cell cell, FieldType fieldType)
    {
        var oldPlant = cell.GetPlantInstance(fieldType);
        Destroy(oldPlant?.gameObject);
        cell.ClearPlant(fieldType);
    }

    public void ToggleGhostPlantVisual(bool show)
    {
        GhostPlant?.gameObject.SetActive(show);
    }
}
