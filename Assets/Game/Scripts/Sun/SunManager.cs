using System;
using UnityEngine;

public class SunManager : Singleton<SunManager>
{
    [SerializeField] private Transform sunCounter;

    public Vector3 SunCounterPos => sunCounter.position;
    public int TotalSun => totalSun;

    public event Action<int> OnSunChanged;

    private int totalSun;

    public void InitSun(int amount)
    {
        totalSun = amount;
    }

    public void CollectSun(int amount)
    {
        totalSun += amount;
        OnSunChanged?.Invoke(totalSun);
    }

    public bool ConsumeSun(int amount)
    {
        if (totalSun < amount) return false;

        totalSun -= amount;
        OnSunChanged?.Invoke(totalSun);
        return true;
    }
}
