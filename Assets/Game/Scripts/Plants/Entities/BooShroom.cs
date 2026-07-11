using UnityEngine;

public class BooShroom : PlantBase
{
    [Header("Events")]
    [SerializeField] private PlantEffectActivateEvent onEffectActivate;
    [SerializeField] private PlantEffectType effectType;

    [Header("References")]
    [SerializeField] private PlantEffect booExplosionPrefab;
    [SerializeField] private Transform booPoint;

    protected override void Die(ZombieBase zombie = null)
    {
        onEffectActivate?.Raise(effectType);

        var effect = Instantiate(booExplosionPrefab, booPoint.transform.position, Quaternion.identity);
        effect.Init(SortingOrderUtility.GetSortingOrder(LayerType.TopPlantEffect, OccupiedCell.Row));
        zombie?.EffectController.ApplyEffect(Data.CreateOnHitEffect(true));
        zombie?.TakeDamage(Data.aoeDamage);
        base.Die(zombie);
    }
}
