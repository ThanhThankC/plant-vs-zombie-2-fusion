public class ZombieContext
{
    public ZombieBase Zombie { get; }
    public ZombieStat Stat { get; }

    public ZombieContext(ZombieBase zombie, ZombieStat stat)
    {
        Zombie = zombie;
        Stat = stat;
    }
}
