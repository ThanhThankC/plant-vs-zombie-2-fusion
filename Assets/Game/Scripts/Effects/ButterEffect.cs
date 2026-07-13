public class ButterEffect : IEffect
{
    public float Duration { get; private set; }

    public ButterEffect() { }

    public ButterEffect(float duration)
    {
        Duration = duration;
    }

    public void OnApply(ZombieContext ctx)
    {
        ctx.Zombie.SpineController.SetSkinActive(AnimEvents.EFFECT_BUTTER, true);
    }

    public void OnExpire(ZombieContext ctx)
    {
        ctx.Zombie.SpineController.SetSkinActive(AnimEvents.EFFECT_BUTTER, false);
    }
}
