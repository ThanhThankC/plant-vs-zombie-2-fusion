using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [System.Serializable]
    private struct PoolEntry
    {
        public PoolKey key;
        public GameObject prefab;
        public int initialCapacity;
    }

    [Header("Plants")]    
    [SerializeField] private PoolEntry[] projectileEntries;
    [SerializeField] private PoolEntry[] splatEntries;
    [SerializeField] private PoolEntry[] plantEntries;
    [SerializeField] private PoolEntry[] plantEffectEntries;
    [Header("Collectibles")]
    [SerializeField] private PoolEntry[] sunEntries;

    [System.Serializable]
    private struct ZombiePoolEntry
    {
        public ZombiePoolKey key;
        public GameObject prefab;
        public int initialCapacity;
    }

    [Header("Zombies")]
    [SerializeField] private ZombiePoolEntry[] zombieEntries;
    [SerializeField] private ZombiePoolEntry[] vfxEntries;

    private readonly Dictionary<PoolKey, ObjectPool<MonoBehaviour>> pools = new();
    private readonly Dictionary<ZombiePoolKey, ObjectPool<MonoBehaviour>> zombiePools = new();

    protected override void Awake()
    {
        base.Awake();

        foreach (var e in projectileEntries) AddPool(e);
        foreach (var e in splatEntries) AddPool(e);
        foreach (var e in plantEntries) AddPool(e);
        foreach (var e in plantEffectEntries) AddPool(e);
        foreach (var e in sunEntries) AddPool(e);
        foreach (var e in zombieEntries) ZombieAddPool(e);
        foreach (var e in vfxEntries) ZombieAddPool(e);
    }

    private void AddPool(PoolEntry e)
    {
        var pool = new ObjectPool<MonoBehaviour>(e.prefab, transform, e.initialCapacity);
        pools.Add(e.key, pool);
    }

    private void ZombieAddPool(ZombiePoolEntry e)
    {
        var pool = new ObjectPool<MonoBehaviour>(e.prefab, transform, e.initialCapacity);
        zombiePools.Add(e.key, pool);
    }

    public T Get<T>(PoolKey key, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        => pools.TryGetValue(key, out var pool)
            ? pool.Get(position, rotation) as T
            : null;

    public void Release<T>(PoolKey key, T obj) where T : MonoBehaviour
    {
        if (pools.TryGetValue(key, out var pool))
            pool.Relase(obj);
    }

    public T GetZombie<T>(ZombiePoolKey key, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        => zombiePools.TryGetValue(key, out var pool)
            ? pool.Get(position, rotation) as T
            : null;

    public void ReleaseZombie<T>(ZombiePoolKey key, T obj) where T : MonoBehaviour
    {
        if (zombiePools.TryGetValue(key, out var pool))
            pool.Relase(obj);
    }
}

public enum PoolKey
{
    //Projectile
    PeaProjectile = 0,
    SnowpeaProjectile = 1,
    FirepeaProjectile = 2,
    GoopeaProjectile = 3,
    CornProjectile = 4,
    ButterProjectile = 5,
    CabbageProjectile = 6,
    MelonProjectile = 7,
    WintermelonProjectile = 8,
    PepperProjectile = 9,

    //Splat VFX
    SplatPea = 100,
    SplatSnowpea = 101,
    SplatFirepea = 102,
    SplatGoopea = 103,
    SplatCorn = 104,
    SplatButter = 105,
    SplatCabbage = 106,
    SplatMelon = 107,
    SplatWintermelon = 108,
    SplatPepper = 109,

    //Plant
    Peashooter = 200,
    Sunflower = 201,
    Wallnut = 202,
    Tallnut = 203,
    FrostFlower = 204,
    Repeater = 205,
    Splitpea = 206,
    Iceberg = 207,
    CherryBomb = 208,
    IceStorm = 209,
    Potatomine = 210,
    Pumpkin = 211,
    Jalapeno = 212,
    BooShroom = 213,
    FirePeashooter = 214,
    Snowpea = 215,
    GooPeashooter = 216,
    Twinflower = 217,
    CabbagePult = 218,
    KernelPult = 219,
    MelonPult = 220,
    WinterMelon = 221,
    PepperPult = 222,
    Spikeweed = 223,
    Garlic = 224,
    ExoNut = 225,
    Edurion = 226,
    PeaVine = 227,

    //Plant Effect
    CherryBombTop = 300,
    CherryBombRear = 301,
    JalapenoFire = 302,
    PotatoExplosion = 303,
    StormFreeze = 304,
    BooExplosion = 305,

    //Other
    Sun = 400,
    BlueSun = 401,
}

public enum ZombiePoolKey
{
    BasicZombie = 0,
    ConeheadZombie = 1,
    BucketheadZombie = 2,
    BrickheadZombie = 3,
    Gargantuar = 4,
    Imp = 5,

    //Zombie Part
    BasicArm = 100,
    BasicHead = 101,
    Buckethead = 102,
    Conehead = 103,
    Brickhead = 104,

    //Zombie VFX
    BasicZombieAsh = 200,
    GargantuarAsh = 201,
    ImpAsh = 202,
}

