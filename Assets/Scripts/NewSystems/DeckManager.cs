using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Tooltip("Assign all 52 CardSO assets here")]
    public List<CardSO> masterDeck = new List<CardSO>();

    private List<CardSO>[] floors = new List<CardSO>[4];
    private bool built = false;

    private void Start()
    {
        BuildAndPartitionDeck();
    }

    [ContextMenu("Build & Partition Deck (Editor)")]
    public void BuildAndPartitionDeck()
    {
        if (built)
        {
            Debug.Log("Deck already built for this run.");
            return;
        }

        built = true;

        if (masterDeck == null || masterDeck.Count != 52)
        {
            Debug.LogWarning("masterDeck should contain 52 cards for a full run. Current: " + (masterDeck?.Count ?? 0));
        }

        var deck = new List<CardSO>(masterDeck);
        Shuffle(deck);

        floors[0] = deck.GetRange(0, Mathf.Min(13, deck.Count));
        floors[1] = deck.GetRange(13, Mathf.Min(13, Mathf.Max(0, deck.Count - 13)));
        floors[2] = deck.GetRange(26, Mathf.Min(14, Mathf.Max(0, deck.Count - 26)));
        floors[3] = deck.GetRange(40, Mathf.Min(14, Mathf.Max(0, deck.Count - 40)));

        Debug.Log($"Deck partitioned: {floors[0].Count}, {floors[1].Count}, {floors[2].Count}, {floors[3].Count}");
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    public List<CardSO> GetFloorDeck(int floorIndex)
    {
        if (!built)
        {
            Debug.LogError("BuildAndPartitionDeck must be called before GetFloorDeck.");
            return null;
        }
        if (floorIndex < 0 || floorIndex >= floors.Length) return null;
        return new List<CardSO>(floors[floorIndex]); 
    }

    public void ResetBuildState()
    {
        built = false;
        floors = new List<CardSO>[4];
    }
}