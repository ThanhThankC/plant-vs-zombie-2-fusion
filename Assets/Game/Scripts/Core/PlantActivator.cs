using System.Collections.Generic;
using UnityEngine;

public enum EffectPlantType { CherryTop, CherryRear, Jalapeno, Potato, Storm}

public class PlantActivator : Singleton<PlantActivator>
{
    [System.Serializable]
    private struct ActivatorEntry
    {
        public PlantType plantType;
        public TargetingHelper targetingHelper;
    }

    [System.Serializable]
    private struct EffectEntry
    {
        public EffectPlantType effectType;
        public PlantEffect prefab;
        public Vector3 spawnOffset;
    }

    [SerializeField] private List<ActivatorEntry> entries;
    private Dictionary<PlantType, ActivatorEntry> actLookup = new();

    [SerializeField] private List<EffectEntry> effects;
    private Dictionary<EffectPlantType, EffectEntry> effectLookup = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in entries)
            actLookup[entry.plantType] = entry;
        foreach (var e in effects)
            effectLookup[e.effectType] = e;
    }

    public void Activate(PlantType plantType, Cell cell, int sortingOrder)
    {
        if (cell == null) return;
        if (!actLookup.TryGetValue(plantType, out var entry)) return;
        switch (plantType)
        {
            case PlantType.CherryBomb:
                EffectPlantResolver.DamageRadius(plantType, cell, entry.targetingHelper);
                SpawnAt(effectLookup[EffectPlantType.CherryTop], cell, LayerType.TopPlantEffect);
                SpawnAt(effectLookup[EffectPlantType.CherryRear], cell, LayerType.RearPlantEffect);
                break;
            case PlantType.Jalapeno:
                EffectPlantResolver.DamageRow(plantType, cell.Row);
                SpawnAtRow(effectLookup[EffectPlantType.Jalapeno], cell, LayerType.TopPlantEffect);
                break;
            case PlantType.PotatoMine:
                EffectPlantResolver.DamageRadius(plantType, cell, entry.targetingHelper);
                SpawnAt(effectLookup[EffectPlantType.Potato],cell, LayerType.TopPlantEffect);
                break;
            case PlantType.IceStorm:
                EffectPlantResolver.FreezeAll(plantType);
                SpawnAt(effectLookup[EffectPlantType.Storm],cell, LayerType.TopPlantEffect);
                break;
        }
    }

    private void SpawnAt(EffectEntry entry, Cell cell, LayerType layerType)
    {
        var effect = Instantiate(entry.prefab, cell.transform.position + entry.spawnOffset , Quaternion.identity);
        effect.Init(SortingOrderUtility.GetSortingOrder(layerType, cell.Row));
    }

    private void SpawnAtRow(EffectEntry entry, Cell cell, LayerType layer)
    {
        for (int col = 0; col < GridManager.Instance.Col; col++)
        {
            var cellSpawn = GridManager.Instance.GetCell(cell.Row, col);
            if (cellSpawn == null) continue;
            SpawnAt(entry, cellSpawn, layer);
        }
    }
}
