using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombieManager : Singleton<ZombieManager>
{
    private readonly List<ZombieBase> activeZombies = new();

    public IReadOnlyList<ZombieBase> ActiveZombies => activeZombies;

    public void RegisterZombie(ZombieBase zombie)
    {
        if (!activeZombies.Contains(zombie))
            activeZombies.Add(zombie);
    }

    public void OnZombieDied(ZombieBase zombie)
    {
        activeZombies.Remove(zombie);
    }
}
