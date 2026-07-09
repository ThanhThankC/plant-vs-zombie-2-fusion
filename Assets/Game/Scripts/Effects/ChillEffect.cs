public class ChillEffect : IEffect
{
    public float Duration { get; private set; }

    public ChillEffect(float duration)
    {
        Duration = duration;
    }

    public void OnApply(ZombieContext ctx)
    {
        ctx.Zombie.VisualHandler.ApplyGlowPreset(GlowType.Freeze);
    }

    public void OnExpire(ZombieContext ctx)
    {
        ctx.Zombie.VisualHandler.ResetGlowPreset();
    }
}
