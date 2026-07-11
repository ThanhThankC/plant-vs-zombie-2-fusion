using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private UnityEvent response;

    private void OnEnable() => gameEvent.OnRaised += OnEventResied;
    private void OnDisable() => gameEvent.OnRaised -= OnEventResied;

    private void OnEventResied()
    {
        response?.Invoke();
    }
}
