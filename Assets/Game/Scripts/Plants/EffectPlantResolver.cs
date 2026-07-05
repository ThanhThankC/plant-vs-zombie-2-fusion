using System.Collections.Generic;
using UnityEngine;

public static class EffectPlantResolver
{
    public static void DamageRow(PlantType plantType, int row)
    {
        var data = PlantManager.Instance.GetPlantData(plantType);
        if (data == null) return;
        foreach (var zombie in new List<ZombieBase>(ZombieManager.Instance.ActiveZombies))
            if (zombie != null && zombie.CellTracker.Row == row)
                zombie.TakeDamage(data.explosionDamage);
    }

    public static void DamageRadius(PlantType plantType, Cell cell, TargetingHelper targetingHelper)
    {
        var data = PlantManager.Instance.GetPlantData(plantType);
        if (data == null) return;
        foreach (var hit in targetingHelper.GetTargetsInRect(cell.transform.position))
        {
            if (!hit.CompareTag("Zombie")) continue;

            var zombie = hit.GetComponent<ZombieBase>();
            if (zombie == null) continue;
            int diffRow = Mathf.Abs(zombie.CellTracker.Row - cell.Row);
            if (diffRow > data.rangeRow) continue;

            zombie.TakeDamage(data.explosionDamage);
        }
    }

    public static void FreezeAll(PlantType plantType)
    {
        var data = PlantManager.Instance.GetPlantData(plantType);
        if (data == null) return;
        foreach (var zombie in new List<ZombieBase>(ZombieManager.Instance.ActiveZombies))
            if (zombie != null) zombie.TakeDamage(data.explosionDamage);
    }
}
