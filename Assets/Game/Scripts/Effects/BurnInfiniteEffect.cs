public class BurnInfiniteEffect : ITickableEffect
{
    public float Duration => -1f;

    public float FirstTickDelay => 1f;

    public float TickInterval => 2f;

    public void OnApply(ZombieContext ctx)
    {
        //if (ctx.Zombie is not IBurnable burnable) return;
        //burnable.FireVisual.SetActive(true);
        //burnable.OnAmrorDestroyed += OnArmorDestroyed;
        //ctx.Zombie.Body.color = ctx.Zombie.FireColor;
        ctx.Zombie.SetBodyColor();
    }

    public void OnTick(ZombieContext ctx)
    {
        ctx.Zombie.TakeDamage(5, DamageSource.Burn);
    }

    public void OnExpire(ZombieContext ctx)
    {
        //if (ctx.Zombie is not IBurnable burnable) return;
        //burnable.FireVisual.SetActive(false);
        //burnable.OnAmrorDestroyed -= OnArmorDestroyed;
        //ctx.Zombie.Body.color = ctx.Zombie.DefaultColor;
        ctx.Zombie.SetBodyColor();
    }

    private void OnArmorDestroyed(ZombieEffectController effectController)
    {
        effectController?.RemoveEffect(typeof(BurnInfiniteEffect));
    }
}
