using System;
using UnityEngine;

public class GameEventChannel<T> : ScriptableObject
{
    public event Action<T> OnRaised;

    public void Raise(T value)
    {
        OnRaised?.Invoke(value);
    }
}
