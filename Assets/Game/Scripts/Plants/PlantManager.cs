using System.Collections.Generic;
using System;
using UnityEngine;

public class PlantManager : Singleton<PlantManager>
{
    [Serializable]
    private struct PlantEntry
    {
        public PlantData data;
        public PoolKey plantKey;
    }

    [SerializeField] private PlantEntry[] plantEntries;

    [Header("Events")]
    [SerializeField] private SunCollectedEvent onSunCollected;

    public GameObject GhostPlant => ghostPlant?.gameObject;
    public PlantType? CurrentPlantType => dragContext?.PlantType;
    public IReadOnlyList<PlantData> AllPlantData => dataList;

    private Dictionary<PlantType, PoolKey> plantKeyLookup = new();
    private Dictionary<PlantType, PlantData> dataLookup = new();
    private List<PlantData> dataList = new();
    private DragContext dragContext;
    private PlantBase ghostPlant;

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in plantEntries)
        {
            plantKeyLookup[entry.data.plantType] = entry.plantKey;
            dataLookup[entry.data.plantType] = entry.data;
            dataList.Add(entry.data);
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
                PlantActivator.Instance.Activate(ctx.PlantType, cell);
                PoolManager.Instance.Release(ctx.Plant.PlantKey, ctx.Plant);
                DestroyPlantAt(cell, result.GetFieldType());
            }
            ctx.SourceCell.ClearPlant(ctx.SourceFieldType);
        }
        else if (ctx.DragSource == DragSource.Deck)
        {
            if (isFusion)
            {
                PlantActivator.Instance.Activate(ctx.PlantType, cell);
                DestroyPlantAt(cell, result.GetFieldType());
            }

            var card = BattleStarter.Instance.GetCard(dragContext.PlantType);
            card?.TriggerCooldown();

            onSunCollected?.Raise(-card.Cost);
        }

        PlantBase plantToPlace;
        if (!isFusion && ctx.DragSource == DragSource.Cell)
        {
            plantToPlace = ctx.Plant;
        }
        else
        {
            if (!plantKeyLookup.TryGetValue(result, out var key)) return;
            plantToPlace = PoolManager.Instance.Get<PlantBase>(key, transform.position, Quaternion.identity);
            plantToPlace.Init(dataLookup[result]);
        }

        plantToPlace.transform.position = cell.transform.position;
        plantToPlace.SetupAsReal(cell, result.GetFieldType());
        cell.SetPlant(result.GetFieldType(), result, plantToPlace);

        EndDrag();
    }

    public void EndDrag()
    {
        if (ghostPlant != null) { PoolManager.Instance.Release(ghostPlant.PlantKey, ghostPlant); ghostPlant = null; }
        dragContext = null;
    }

    public bool IsDraggingFromCell(Cell cell) => dragContext?.DragSource == DragSource.Cell && dragContext?.SourceCell == cell;

    public void DestroyPlantAt(Cell cell, FieldType fieldType)
    {
        var oldPlant = cell.GetPlantInstance(fieldType);
        PoolManager.Instance.Release(oldPlant.PlantKey, oldPlant);
        cell.ClearPlant(fieldType);
    }

    public void ToggleGhostPlantVisual(bool show)
    {
        ghostPlant?.gameObject.SetActive(show);
    }

    private void SpawnGhost(PlantType plantType, Cell cell = null)
    {
        if (!plantKeyLookup.TryGetValue(plantType, out var key)) return;
        ghostPlant = PoolManager.Instance.Get<PlantBase>(key, transform.position, Quaternion.identity);
        ghostPlant.SetupAsGhost(cell, plantType.GetFieldType());
    }

    public PlantData GetPlantData(PlantType plantType) => dataLookup[plantType];
}
