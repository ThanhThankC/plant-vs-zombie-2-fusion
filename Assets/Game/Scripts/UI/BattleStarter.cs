using System.Collections.Generic;

public class BattleStarter : Singleton<BattleStarter>
{
    private Dictionary<PlantType, CardPlant> allCards = new Dictionary<PlantType, CardPlant>();
    private bool battleStarted = false;

    private void Start() => GameFlowManager.Instance.OnStateChanged += OnStartBattle;

    private void OnDisable()
    {
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.OnStateChanged -= OnStartBattle;
    }

    public void RegisterCard(CardPlant card, PlantType plantType)
    {
        if (!allCards.ContainsKey(plantType))
            allCards[plantType] = card;
    }

    public CardPlant GetCard(PlantType plantType)
    {
        allCards.TryGetValue(plantType, out CardPlant card);
        return card;
    }

    private void OnStartBattle(GameState gameState)
    {
        if (gameState == GameState.Playing)
        {
            if (battleStarted) return;
            battleStarted = true;

            foreach (var card in allCards.Values)
            {
                card.EnableBattle();
                card.SelectionCard.enabled = false;
            }
        }
    }
}