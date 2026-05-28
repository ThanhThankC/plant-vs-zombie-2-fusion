public class FreezeEffect : IEffect
{
    public float Duration => 3f;

    public void OnApply(ZombieContext ctx)
    {
        //ctx.Zombie.Body.color = ctx.Zombie.IceColor;
        //ctx.Zombie.IceEffect.SetActive(true);
    }

    public void OnExpire(ZombieContext ctx)
    {
        //ctx.Zombie.Body.color = ctx.Zombie.DefaultColor;
        //ctx.Zombie.IceEffect.SetActive(false);
    }
}