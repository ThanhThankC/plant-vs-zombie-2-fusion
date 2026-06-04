using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombieManager : Singleton<ZombieManager>
{
    [Serializable]
    private struct ZombieEntry
    {
        public ZombieData data;
        public ZombieBase prefab;
    }

    [SerializeField] private List<ZombieEntry> zombieEntries;

    private Dictionary<ZombieType, ZombieBase> prefabLookup = new();
    private Dictionary<ZombieType, ZombieData> dataLookup = new();
    private readonly List<ZombieBase> activeZombies = new();

    public IReadOnlyList<ZombieBase> ActiveZombies => activeZombies;

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in zombieEntries)
        {
            prefabLookup[entry.data.zombieType] = entry.prefab;
            dataLookup[entry.data.zombieType] = entry.data;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            Spawn(ZombieType.Basic, 0);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            Spawn(ZombieType.Basic, 1);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            Spawn(ZombieType.Basic, 2);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Spawn(ZombieType.Basic, 3);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            Spawn(ZombieType.Basic, 4);
    }

    public ZombieBase Spawn(ZombieType zombieType, int row)
    {
        if (!prefabLookup.TryGetValue(zombieType, out var prefab))
        {
            Debug.LogWarning($"[ZombieManager] Zombie type not found: {zombieType}!");
            return null;
        }
        
        var spawnCell = GridManager.Instance.GetCell(row, GridManager.ZombieSpawnCol);
        if (spawnCell == null) return null;

        var zombie = Instantiate(prefabLookup[zombieType], spawnCell.transform.position, Quaternion.identity, transform);
        zombie.Init(dataLookup[zombieType]);
        zombie.CellTracker.Init(row, GridManager.ZombieSpawnCol);

        RegisterZombie(zombie);
        return zombie;
    }

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
