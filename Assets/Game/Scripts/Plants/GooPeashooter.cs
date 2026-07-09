using UnityEngine;

public class GooPeashooter : Peashooter
{
    [Header("Explosion")]
    [SerializeField] private PlantEffect booExplosionPrefab;
    [SerializeField] private Transform booPoint;

    protected override void Die(ZombieBase zombie = null)
    {
        var effect = Instantiate(booExplosionPrefab, booPoint.transform.position, Quaternion.identity);
        effect.Init(SortingOrderUtility.GetSortingOrder(LayerType.TopPlantEffect, OccupiedCell.Row));
        zombie?.EffectController.ApplyEffect(Data.CreateOnHitEffect(true));
        zombie?.TakeDamage(Data.aoeDamage);
        base.Die(zombie);
    }
}
