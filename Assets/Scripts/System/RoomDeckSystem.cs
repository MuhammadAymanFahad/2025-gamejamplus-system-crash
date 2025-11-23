using System.Collections.Generic;
using UnityEngine;

public class RoomDeckManager : MonoBehaviour
{
    public static RoomDeckManager Instance { get; private set; }

    public List<RoomCardData> allRoomCards; // Assign di Inspector

    private List<RoomCardData> activeDeck = new List<RoomCardData>();
    private List<RoomCardData> discardPile = new List<RoomCardData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeRoomDeck()
    {
        activeDeck = new List<RoomCardData>(allRoomCards);
        discardPile.Clear();
        ShuffleDeck();
        Debug.Log($"[RoomDeck] Initialized with {activeDeck.Count} cards");
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < activeDeck.Count; i++)
        {
            RoomCardData temp = activeDeck[i];
            int randomIndex = Random.Range(i, activeDeck.Count);
            activeDeck[i] = activeDeck[randomIndex];
            activeDeck[randomIndex] = temp;
        }
    }

    public List<RoomCardData> DrawRoomCards(int count)
    {
        List<RoomCardData> drawn = new List<RoomCardData>();

        for (int i = 0; i < count; i++)
        {
            if (activeDeck.Count > 0)
            {
                drawn.Add(activeDeck[0]);
                activeDeck.RemoveAt(0);
            }
        }

        Debug.Log($"[RoomDeck] Drew {drawn.Count} cards");
        return drawn;
    }
}
