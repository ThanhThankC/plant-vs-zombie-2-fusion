public interface ITickableEffect : IEffect
{
    float FirstTickDelay { get; }
    float TickInterval { get; }
    void OnTick(ZombieContext ctx);
}
