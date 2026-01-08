using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour {
    [Tooltip("Assign all 52 CardSO assets here")] 

    public List<CardSO> masterDeck = new List<CardSO>();
    private List<CardSO>[] floors = new List<CardSO>[4];

    public void BuildAndPartitionDeck()
    {
        if (masterDeck == null || masterDeck.Count != 52)
        {
            Debug.LogWarning("Master deck must contain exactly 52 cards. Current: \" + (masterDeck?.Count ?? 0)");
            return;
        }

        var deck = new List<CardSO>(masterDeck);
        Shuffle(deck);

        floors[0] = deck.GetRange(0, 13);
        floors[1] = deck.GetRange(13, 13);
        floors[2] = deck.GetRange(26, 14);
        floors[3] = deck.GetRange(40, 14);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public List<CardSO> GetFloorDeck(int floorIndex)
    {
        if(floorIndex < 0 || floorIndex >= floors.Length)
        {
            return null;
        }
        return new List<CardSO>(floors[floorIndex]);
    }
}
