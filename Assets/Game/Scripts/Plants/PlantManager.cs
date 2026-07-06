using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlantManager : Singleton<PlantManager>
{
    [Serializable]
    private struct PlantEntry
    {
        public PlantData data;
        public PlantBase prefab;
    }

    [SerializeField] private List<PlantEntry> plantEntries;
    public GameObject GhostPlant => ghostPlant?.gameObject;
    public PlantType? CurrentPlantType => dragContext?.PlantType;

    private Dictionary<PlantType, PlantBase> prefabLookup = new();
    private Dictionary<PlantType, PlantData> dataLookup = new();
    private DragContext dragContext;
    private PlantBase ghostPlant;

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
        dragContext = new DragContext
        {
            DragSource = DragSource.Deck,
            PlantType = plantType
        };
        SpawnGhost(plantType);
    }

    public void OnCellClicked(PlantBase plant, Cell cell)
    {
        dragContext = new DragContext
        {
            Plant = plant,
            DragSource = DragSource.Cell,
            SourceCell = cell,
            PlantType = plant.PlantType
        };
        SpawnGhost(plant.PlantType, cell);
    }

    public void PlacePlant(Cell cell, PlantType result)
    {
        DragContext ctx = dragContext;
        bool isFusion = ctx.PlantType != result;

        if (ctx.DragSource == DragSource.Cell)
        {
            if (isFusion)
            {
                PlantActivator.Instance.Activate(ctx.PlantType, cell, 10);
                Destroy(ctx.Plant.gameObject);
            }
            ctx.SourceCell.ClearPlant(ctx.SourceFieldType);
        }

        if (isFusion)
        {
            PlantActivator.Instance.Activate(ctx.PlantType, cell, 10);
            DestroyPlantAt(cell, result.GetFieldType());
        }

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
        plantToPlace.SetupAsReal(cell, result.GetFieldType());
        cell.SetPlant(result.GetFieldType(), result, plantToPlace);

        EndDrag();
    }

    public void EndDrag()
    {
        if (ghostPlant != null) { Destroy(ghostPlant.gameObject); ghostPlant = null; }
        dragContext = null;
    }

    public bool IsDraggingFromCell(Cell cell) => dragContext?.DragSource == DragSource.Cell && dragContext?.SourceCell == cell;

    public void DestroyPlantAt(Cell cell, FieldType fieldType)
    {
        var oldPlant = cell.GetPlantInstance(fieldType);
        Destroy(oldPlant.gameObject);
        cell.ClearPlant(fieldType);
    }

    public void ToggleGhostPlantVisual(bool show)
    {
        ghostPlant?.gameObject.SetActive(show);
    }

    private void SpawnGhost(PlantType plantType, Cell cell = null)
    {
        if (!prefabLookup.TryGetValue(plantType, out var prefab)) return;
        ghostPlant = Instantiate(prefab, transform);
        ghostPlant.SetupAsGhost(cell, plantType.GetFieldType());
    }

    public PlantData GetPlantData(PlantType plantType) => dataLookup[plantType];
}
