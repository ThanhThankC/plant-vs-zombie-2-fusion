using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField] private List<WaveData> waveDatas;
    //TODO: Variable Conventions
    [SerializeField] private int recentRowMemorySize = 3;
    [SerializeField] private float progressSize = 1.1f;

    public int BigWaveCount { get; private set; }

    public event System.Action<float, int> OnWaveChanged;

    private List<ZombieBase> lastSmallWaveZombies = new();
    private Queue<int> rowQueue = new();
    private WaveData currentWaveData;
    private int smallWaveCount;
    private int bigWaveIndex;
    private int smallWaveIndex;
    private int currentLevel;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        StartLevel(currentLevel);
    }

    private void StartLevel(int level)
    {
        currentWaveData = waveDatas[level];
        if (currentWaveData == null) return;
        StartCoroutine(RunWave());
    }

    IEnumerator RunWave()
    {
        yield return new WaitForSeconds(currentWaveData.prepareTime);

        BigWaveCount = currentWaveData.bigWaves.Count;
        for (int i = 0; i < BigWaveCount; i++)
        {
            var bigwave = currentWaveData.bigWaves[i];
            yield return RunBigWave(bigwave.smallWaves);
            smallWaveIndex = 0;
            bigWaveIndex++;
        }
    }

    IEnumerator RunBigWave(List<SmallWave> smallWaves)
    {
        smallWaveCount = smallWaves.Count;
        for (int i = 0; i < smallWaves.Count; i++)
        {
            OnWaveChanged?.Invoke(CalculateProgress(), bigWaveIndex);
            yield return RunSmallWave(smallWaves[i]);
            yield return WaitingForNextSmallWave(smallWaves[i]);
            smallWaveIndex++;
        }
    }

    IEnumerator RunSmallWave(SmallWave smallWave)
    {
        lastSmallWaveZombies.Clear();
        for (int i = 0; i < smallWave.spawnCount; i++)
        {
            int row = PickRow();
            var zombie = ZombieManager.Instance.Spawn(PickZombieType(smallWave.entries), row);
            if (zombie != null) lastSmallWaveZombies.Add(zombie);
            yield return new WaitForSeconds(smallWave.spawnInterval);
        }
    }

    IEnumerator WaitingForNextSmallWave(SmallWave smallWave)
    {
        float elapsed = 0;
        while (elapsed < smallWave.maxWaveDuration)
        {
            if (AnyZombieDied(lastSmallWaveZombies)) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private ZombieType PickZombieType(List<ZombieSpawnEntry> entries)
    {
        var sorted = entries.ToList();
        sorted.Sort((a, b) => b.weight.CompareTo(a.weight));

        int totalWeight = 0;
        foreach (var e in sorted) totalWeight += e.weight;

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var e in sorted)
        {
            cumulative += e.weight;
            if (roll < cumulative) return e.zombieType;
        }
        return entries[entries.Count - 1].zombieType;
    }

    private int PickRow()
    {
        //TODO: Variable Conventions
        if (recentRowMemorySize >= 5) return Random.Range(0, 5);

        var available = new List<int>();
        for (int i = 0; i < 5; i++)
            if (!rowQueue.Contains(i)) available.Add(i);

        int roll = available[Random.Range(0, available.Count)];
        rowQueue.Enqueue(roll);

        if (rowQueue.Count > recentRowMemorySize)
            rowQueue.Dequeue();

        return roll;
    }

    private bool AnyZombieDied(List<ZombieBase> zombies)
    {
        foreach (var z in zombies)
            if (z == null || z.IsDead) return true;
        return false;
    }

    private float CalculateProgress()
    {
        if (bigWaveIndex + 1 >= BigWaveCount) return 1f;

        float perBigProgress = progressSize / (BigWaveCount - 1);

        float progress = smallWaveIndex * perBigProgress / smallWaveCount
                        + bigWaveIndex * perBigProgress;
        return progress;
    }
}
