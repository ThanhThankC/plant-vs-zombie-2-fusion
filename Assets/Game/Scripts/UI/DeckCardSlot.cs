using System.Collections.Generic;
using UnityEngine;

public class DeckCardSlot : MonoBehaviour
{
    [SerializeField] private GameObject emptySlotPrefab;
    [SerializeField] private int totalSlot = 8;

    private struct SlotState
    {
        public Transform transform;
        public bool reserved;
    }

    private List<SlotState> slots = new();

    private void Start()
    {
        for (int i = 0; i < totalSlot; i++)
        {
            var slot = Instantiate(emptySlotPrefab, transform);

            slots.Add(new SlotState { transform = slot.transform, reserved = false });
        }
    }

    public Transform ClaimFirstEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].reserved)
            {
                var s = slots[i];
                s.reserved = true;
                slots[i] = s;
                return slots[i].transform;
            }
        }
        return null;
    }

    public void ReleaseSlot(Transform slot)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform == slot)
            {
                var s = slots[i];
                s.reserved = false;
                slots[i] = s;
                return;
            }
        }
    }

    public void CompactSlots()
    {
        List<Transform> cards = new();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
                cards.Add(slots[i].transform.GetChild(0));
        }

        foreach (var card in cards)
            card.SetParent(null, worldPositionStays: true);

        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i];
            s.reserved = false;
            slots[i] = s;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetParent(slots[i].transform, worldPositionStays: false);
            cards[i].localPosition = Vector3.zero;
            cards[i].localScale = Vector3.one;
            var s = slots[i];
            s.reserved = true;
            slots[i] = s;
        }
    }
}