using UnityEngine;

[RequireComponent(typeof(TargetingHelper))]
public class PultProjectileAoE : PultProjectile
{
    private TargetingHelper targetingHelper;

    protected override void Awake()
    {
        base.Awake();
        targetingHelper = GetComponent<TargetingHelper>();
    }

    protected override void OnHit()
    {
        foreach (var hit in targetingHelper.GetTargetsInRect(transform.position))
        {
            if (!hit.CompareTag("Zombie")) continue;

            var zombie = hit.GetComponent<ZombieBase>();
            if (zombie == null) continue;
            int diffRow = Mathf.Abs(zombie.CellTracker.Row - spawnCell.Row);
            if (diffRow > data.rangeRow) continue;

            zombie.EffectController.ApplyEffect(onHitEffect);
            zombie.TakeDamage(data.aoeDamage, data.damageSource);
        }
    }
}
