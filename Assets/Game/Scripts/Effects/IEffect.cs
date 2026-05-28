public interface IEffect
{
    float Duration { get; }
    void OnApply(ZombieContext ctx);
    void OnExpire(ZombieContext ctx);
}
