public class StinkyEffect : ILaneSwitchEffect
{
    public float Duration => -1f;

    public void OnApply(ZombieContext ctx)
    {
        //ctx.Zombie.StinkyEffect.SetActive(true);
    }

    public void OnLaneReached(ZombieContext ctx)
    {
        //ctx.Zombie.EffectController.RemoveEffect(typeof(StinkyEffect));
    }

    public void OnExpire(ZombieContext ctx)
    {
        //ctx.Zombie.StinkyEffect.SetActive(false);
    }
}
