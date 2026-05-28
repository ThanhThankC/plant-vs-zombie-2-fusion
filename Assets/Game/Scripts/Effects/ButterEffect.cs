public class ButterEffect : IEffect
{
    public float Duration => 4f;

    public void OnApply(ZombieContext ctx)
    {
        //ctx.Zombie.ButterEffect.SetActive(true);
    }

    public void OnExpire(ZombieContext ctx)
    {
        //ctx.Zombie.ButterEffect.SetActive(false);
    }
}
