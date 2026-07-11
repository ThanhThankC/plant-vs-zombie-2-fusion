using System;
using UnityEngine;

public class GameEvent : ScriptableObject
{
    public event Action OnRaised;
    public void Raise()
    {
        OnRaised?.Invoke();
    }
}
