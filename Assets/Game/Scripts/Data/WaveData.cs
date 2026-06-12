using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WaveData", menuName ="PVZF/Wave Data")]
public class WaveData : ScriptableObject
{
    public float prepareTime = 10f;
    public List<BigWave> bigWaves;
}

[System.Serializable]
public class BigWave
{
    public List<SmallWave> smallWaves;
}

[System.Serializable]
public class SmallWave
{
    public int spawnCount;
    public float maxWaveDuration = 20f;
    public float spawnInterval = 2f;
    public List<ZombieSpawnEntry> entries;
}

[System.Serializable]
public class ZombieSpawnEntry
{
    public ZombieType zombieType;
    [Range(0, 100)] public int weight;
}