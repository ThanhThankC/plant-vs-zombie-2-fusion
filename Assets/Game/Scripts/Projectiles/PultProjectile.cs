using DG.Tweening;
using UnityEngine;

public class PultProjectile : ProjectileBase
{
    [Header("Not touching zombie")]
    [SerializeField] private bool onlyGround = false;

    private Vector3 targetPosition;

    public override void Init(Vector3 target, Cell cell, IEffect effect = null)
    {
        base.Init(target, cell, effect);
        targetPosition = target;
        LaunchParabola();
    }

    private void LaunchParabola()
    {
        transform.DOJump(targetPosition, data.arcHeight, 1, data.arcDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(() => UpdateShadow())
            .OnComplete(() => OnSplat());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isReturned || onlyGround) return;
        if (!collider.CompareTag("Zombie")) return;

        var zombie = collider.GetComponent<ZombieBase>();
        if (zombie == null || zombie.IsDead || zombie.CellTracker.Row != spawnCell.Row) return;

        if (onHitEffect != null)
            zombie.EffectController.ApplyEffect(onHitEffect);

        zombie.TakeDamage(data.damage, data.damageSource);

        OnSplat();
    }

    private void OnSplat()
    {
        OnHit();
        DOTween.Kill(transform);
        ReturnPool();
    }

    protected virtual void OnHit() { }
}
