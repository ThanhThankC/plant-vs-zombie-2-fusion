using UnityEngine;

public class PeaProjectile : ProjectileBase
{
    private Vector3 direction = Vector3.right;

    public override void Init(Vector3 direction, Cell cell, IEffect effect = null)
    {
        base.Init(direction, cell, effect);
        this.direction = direction;
        onHitEffect = effect;
    }

    void Update()
    {
        if (direction == Vector3.left && transform.position.x < GridManager.Instance.GetHousePositionX())
            direction = Vector3.right;

        transform.Translate(direction * data.linearSpeed * Time.deltaTime);

        UpdateShadow();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Zombie")) return;

        var zombie = other.GetComponent<ZombieBase>();
        if (zombie == null || zombie.IsDead || zombie.CellTracker.Row != spawnCell.Row) return;

        if (onHitEffect != null)
            zombie.EffectController.ApplyEffect(onHitEffect);

        zombie.TakeDamage(data.damage, data.damageSource);

        OnImpact();
        Destroy(gameObject);
    }
}
