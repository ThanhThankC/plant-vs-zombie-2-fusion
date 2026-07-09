public class FreezeEffect : IEffect
{
    public float Duration { get; private set; }
    public float ChillDuration { get; private set; }

    public FreezeEffect(float freezeDuration, float chillDuration)
    {
        Duration = freezeDuration;
        ChillDuration = chillDuration;
    }

    public void OnApply(ZombieContext ctx)
    {
        ctx.Zombie.VisualHandler.ApplyGlowPreset(GlowType.Freeze);
        ctx.Zombie.VisualHandler.FreezeEffect.SetActive(true);
    }

    public void OnExpire(ZombieContext ctx)
    {
        ctx.Zombie.VisualHandler.ResetGlowPreset();
        ctx.Zombie.VisualHandler.FreezeEffect.SetActive(false);
    }
}