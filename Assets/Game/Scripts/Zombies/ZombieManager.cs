using System.Collections.Generic;
using UnityEngine;
using System;

public class ZombieManager : Singleton<ZombieManager>
{
    [Serializable]
    private struct ZombieEntry
    {
        public ZombieData data;
        public ZombiePoolKey zombieKey;
    }

    [Header("Events")]
    [SerializeField] private ZombieDiedEvent onZombieDied;
    [SerializeField] private LevelClearedEvent onLevelCleared;

    [Header("References")]
    [SerializeField] private List<ZombieEntry> zombieEntries;
    [SerializeField] private float distanceSpawn = 3f;

    public IReadOnlyList<ZombieBase> ActiveZombies => activeZombies;

    private Dictionary<ZombieType, ZombiePoolKey> keyLookup = new();
    private Dictionary<ZombieType, ZombieData> dataLookup = new();
    private readonly List<ZombieBase> activeZombies = new();

    private bool allWavesSpawned = false;

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in zombieEntries)
        {
            keyLookup[entry.data.zombieType] = entry.zombieKey;
            dataLookup[entry.data.zombieType] = entry.data;
        }
    }

    private void OnEnable() => onZombieDied.OnRaised += OnZombieDied;
    private void OnDisable() => onZombieDied.OnRaised -= OnZombieDied;

    public void NotifyAllWavesSpawned()
    {
        allWavesSpawned = true;
        CheckLevelCleared();  
    }

    public ZombieBase Spawn(ZombieType zombieType, int row)
    {
        if (!keyLookup.TryGetValue(zombieType, out var key))
        {
            Debug.LogWarning($"[ZombieManager] Zombie type not found: {zombieType}!");
            return null;
        }

        var spawnCell = GridManager.Instance.GetCell(row, GridManager.ZombieSpawnCol);
        if (spawnCell == null) return null;

        Vector3 spawnPos = spawnCell.transform.position + new Vector3(distanceSpawn, 0f, 0f);

        var zombie = PoolManager.Instance.GetZombie<ZombieBase>(key, spawnPos, Quaternion.identity);
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

    private void CheckLevelCleared()
    {
        if (!allWavesSpawned) return;
        if (activeZombies.Count != 0) return;
        onLevelCleared?.Raise();
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

    public ZombieBase GetNearestZombieInRow(int row, float fromPosX)
    {
        if (activeZombies.Count == 0) return null;

        ZombieBase nearestZombie = null;
        float nearestDistance = float.MaxValue;

        foreach (var zombie in activeZombies)
        {
            if (zombie == null) continue;

            var cellTracker = zombie.GetComponentInChildren<CellTracker>();
            if (cellTracker == null) continue;

            if (cellTracker.Row == row && fromPosX <= zombie.transform.position.x)
            {
                float distance = zombie.transform.position.x - fromPosX;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestZombie = zombie;
                }
            }
        }

        return nearestZombie;
    }
}
