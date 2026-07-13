using UnityEngine;
using System.Collections.Generic;

public class ZombiePreviewManager : Singleton<ZombiePreviewManager>
{
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Vector2 offsetRange = new Vector2(2f, 1f);

    private readonly List<(ZombiePoolKey key, ZombieBase zombie)> activeZombies = new();

    private void Start() => GameFlowManager.Instance.OnStateChanged += OnStateChanged;

    private void OnDisable()
    {
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.CardSelection)
            ShowAll();
        else if (state == GameState.BattleIntro)
            HideAll();
    }

    private void ShowAll()
    {
        var previews = WaveManager.Instance.WaveLevelData.previews;
        int count = previews.Count;

        for (int i = 0; i < count; i++)
        {
            float t = count > 1 ? (float)i / (count - 1) : 0.5f;
            float y = Mathf.Lerp(offsetRange.y, -offsetRange.y, t);

            var pos = centerPoint.position + new Vector3( Random.Range(-offsetRange.x, offsetRange.x), y, 0f );

            var zombie = PoolManager.Instance.GetZombie<ZombieBase>(previews[i], pos, Quaternion.identity);
            zombie.SetGhostZombie(true);
            zombie.AnimController.PlayIdle();
            activeZombies.Add((previews[i], zombie));
        }
    }

    private void HideAll()
    {
        foreach (var (key, zombie) in activeZombies)
            PoolManager.Instance.ReleaseZombie(key, zombie);

        activeZombies.Clear();
    }
}