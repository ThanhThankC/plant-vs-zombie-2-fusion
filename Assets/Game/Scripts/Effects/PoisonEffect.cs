using UnityEngine;

public class PoisonEffect : ITickableEffect
{
    public float Duration { get; private set; }

    public float FirstTickDelay => 2f;

    public float TickInterval => 3f;

    private const int maxDamage = 30;
    private int damage;

    public PoisonEffect(int damagePerTick, float duration)
    {
        damage = damagePerTick;
        Duration = duration;
    }

    public void OnApply(ZombieContext ctx)
    {
        ctx.Zombie.SpineController.SetSkinActive(AnimEvents.EFFECT_POISON, true);
        ctx.Zombie.VisualHandler.ApplyGlowPreset(GlowType.Poison);
    }

    public void OnTick(ZombieContext ctx)
    {
        ctx.Stat.PoisonDamageTick = Mathf.Min(ctx.Stat.PoisonDamageTick + damage, maxDamage);
        ctx.Zombie.TakeDamage(ctx.Stat.PoisonDamageTick, DamageSource.Normal);
    }

    public void OnExpire(ZombieContext ctx)
    {
        ctx.Zombie.SpineController.SetSkinActive(AnimEvents.EFFECT_POISON, false);
        ctx.Zombie.VisualHandler.ResetGlowPreset();
        ctx.Stat.PoisonDamageTick = 0;
    }
}
