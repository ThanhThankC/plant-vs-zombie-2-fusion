using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField] private List<WaveData> waveDatas;
    //TODO: Variable Conventions
    [SerializeField] private int recentRowMemorySize = 3;

    public float WaveProgress => CaculateProgress();

    public event System.Action<float> OnWaveChanged;

    private List<ZombieBase> lastSmallWaveZombies = new();
    private Queue<int> rowQueue = new();
    private WaveData currentWaveData;
    private int currentLevel;
    private int currentBigWaveCount;
    private int currentSamllWaveCount;
    private int bigWaveIndex;
    private int smallWaveIndex;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        currentWaveData = waveDatas[currentLevel];
        if (currentWaveData == null) return;
        StartCoroutine(RunWave());
    }

    IEnumerator RunWave()
    {
        yield return new WaitForSeconds(currentWaveData.prepareTime);

        currentBigWaveCount = currentWaveData.bigWaves.Count;
        for (int i = 0; i < currentBigWaveCount; i++)
        {
            var bigwave = currentWaveData.bigWaves[i];
            yield return RunBigWave(bigwave.smallWaves);
            bigWaveIndex++;
        }
    }

    IEnumerator RunBigWave(List<SmallWave> smallWaves)
    {
        currentSamllWaveCount = smallWaves.Count;
        for (int i = 0; i < smallWaves.Count; i++)
        {
            yield return RunSmallWave(smallWaves[i]);
            yield return WaitingForNextSmallWave(smallWaves[i]);
            smallWaveIndex++;
            OnWaveChanged?.Invoke(CaculateProgress());
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
        if (AnyZombieDied(lastSmallWaveZombies)) yield break;
        yield return new WaitForSeconds(smallWave.maxWaveDuration);
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

    private float CaculateProgress()
    {
        float progress = ( smallWaveIndex / (float) currentSamllWaveCount) 
                       * ( bigWaveIndex + 1 / (float) currentBigWaveCount);
        return progress;
    }
}
