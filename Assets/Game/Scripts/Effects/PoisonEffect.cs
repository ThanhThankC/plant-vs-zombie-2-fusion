using UnityEngine;

public class PoisonEffect : ITickableEffect
{
    public float Duration => 5f;

    public float FirstTickDelay => 2f;

    public float TickInterval => 3f;

    public void OnApply(ZombieContext ctx)
    {
        //ctx.Zombie.PoisonBodyEffect.SetActive(true);
        //ctx.Zombie.PoisonHeadEffect.SetActive(true);
    }

    public void OnTick(ZombieContext ctx)
    {
        ctx.Stat.PoisonDamageTick = Mathf.Min(ctx.Stat.PoisonDamageTick + 5, 30);
        ctx.Zombie.TakeDamage(ctx.Stat.PoisonDamageTick, DamageSource.Normal);
    }

    public void OnExpire(ZombieContext ctx)
    {
        //ctx.Zombie.PoisonBodyEffect.SetActive(false);
        //ctx.Zombie.PoisonHeadEffect.SetActive(false);
        //ctx.Stat.PoisonDamageTick = 0;
    }
}
