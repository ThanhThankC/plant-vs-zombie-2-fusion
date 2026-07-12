using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private CardPlant cardPrefab;
    [SerializeField] private int[] enableTiers;
    [SerializeField] private PlantType[] bannedPlants;

    [Header("References")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private DeckCardSlot deck;
    [SerializeField] private GameObject slotWrapper;

    private void Start()
    {
        foreach (var data in PlantManager.Instance.AllPlantData)
        {
            bool tierOk = false;
            foreach (var tier in enableTiers)
            {
                if (data.tier == tier) { tierOk = true; break; }
            }
            if (!tierOk) continue;

            bool banned = false;
            foreach (var ban in bannedPlants)
            {
                if (data.plantType == ban) { banned = true; break; }
            }
            if (banned) continue;

            var slot = Instantiate(new GameObject(), slotWrapper.transform);
            slot.AddComponent<RectTransform>();

            var card = Instantiate(cardPrefab, slot.transform);
            card.Init(data);
            card.SelectionCard.Init(rootCanvas, deck, slot.transform);

            var ghostCard = Instantiate(cardPrefab, slot.transform);
            ghostCard.Init(data);
            ghostCard.SelectionCard.Init(true);
            ghostCard.transform.SetAsFirstSibling();
        }
    }
}
