using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ZombieManager : Singleton<ZombieManager>
{
    [Serializable]
    private struct ZombieEntry
    {
        public ZombieData data;
        public ZombieBase prefab;
    }

    [SerializeField] private List<ZombieEntry> zombieEntries;

    public IReadOnlyList<ZombieBase> ActiveZombies => activeZombies;

    private Dictionary<ZombieType, ZombieBase> prefabLookup = new();
    private Dictionary<ZombieType, ZombieData> dataLookup = new();
    private readonly List<ZombieBase> activeZombies = new();

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

    public bool HasZombieInRow(int row, float fromPosX)
    {
        if (activeZombies.Count == 0) return false;

        foreach (var zombie in activeZombies)
        {
            if (zombie == null) continue;
            var cellTracker = zombie.GetComponentInChildren<CellTracker>();
            if (cellTracker.Row == row && fromPosX <= zombie.transform.position.x)
            {
                return true;
            }
        }
        return false;
    }
}
