using TMPro;
using UnityEngine;

public class PlantPreviewManager : Singleton<PlantPreviewManager>
{
    [SerializeField] private TextMeshProUGUI plantName;
    [SerializeField] private TextMeshProUGUI plantDescription;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PoolKey template = PoolKey.Peashooter;
    [SerializeField] private PlantData templateData;

    private PlantBase currentPlant;
    private PoolKey currentKey;

    private void Start() => GameFlowManager.Instance.OnStateChanged += InitPreview;

    private void OnDisable()
    {
        if (GameFlowManager.Instance != null)
        GameFlowManager.Instance.OnStateChanged -= InitPreview;
    }

    private void InitPreview(GameState state)
    {
        if (state == GameState.CardSelection)
            Show(template, templateData);
        else if (state == GameState.BattleIntro)
            Hide();
    }

    public void Show(PoolKey key, PlantData data)
    {
        if (currentPlant != null)
        {
            if (currentKey.Equals(key)) return;
            Hide();
        }

        currentKey = key;
        currentPlant = PoolManager.Instance.Get<PlantBase>(key, spawnPoint.position, Quaternion.identity);

        currentPlant.OnDisplay();

        if (plantName != null) plantName.text = data.plantName;
        if (plantDescription != null) plantDescription.text = data.description;

    }

    private void Hide()
    {
        if (currentPlant == null) return;
        PoolManager.Instance.Release(currentKey, currentPlant);
        currentPlant = null;
    }
}