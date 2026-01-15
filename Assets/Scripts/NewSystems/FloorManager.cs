using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public int currentFloorIndex = 0;
    public RoomState[] rooms = new RoomState[4];
    private DeckManager deckManager;
    private List<CardSO> currentFloorDeck;

    private int escapeTryCount = 0;

    void Awake()
    {
        deckManager = FindObjectOfType<DeckManager>();
        if(deckManager == null)
        {
            Debug.LogError("DeckManager not found in the scene.");
        }
    }

    public void SetupFloor(int floorIndex)
    {
        currentFloorIndex = floorIndex;
        var floorDeck = deckManager.GetFloorDeck(floorIndex);
        if (floorDeck == null)
        {
            Debug.LogError("No floor deck available. Make sure BuildAndPartitionDeck was called.");
            return;
        }

        rooms = new RoomState[4];
        int cardIndex = 0;
        for (int r = 0; r < 4; r++)
        {
            int take = Mathf.Min(4, Mathf.Max(0, floorDeck.Count - cardIndex));
            var four = floorDeck.GetRange(cardIndex, take);
            rooms[r] = new RoomState(four);
            cardIndex += take;
        }

        Debug.Log($"Floor {floorIndex} setup: rooms prepared with counts: {rooms[0].cards.Count}, {rooms[1].cards.Count}, {rooms[2].cards.Count}, {rooms[3].cards.Count}");
    }

    public bool TryEscape()
    {
        float[] chances = { 1f, 0.66f, 0.33f, 0f };
        if (escapeTryCount >= chances.Length )
        {
            escapeTryCount = chances.Length - 1;
        }
        float p = chances[Mathf.Clamp(escapeTryCount, 0, chances.Length - 1)];
        float roll = Random.value;
        bool success = roll <= p;

        escapeTryCount++;
        Debug.Log($"Escape attemt {escapeTryCount} rolled {roll:F2} vs chance {p:F2} => success={success}"); 
        return success;
    }

    public void RestEscapeAttempts()
    {
        this.escapeTryCount = 0;
    }

    public bool IsFloorCleared()
    {
        foreach (var room in rooms)
        {
            if(room == null)
            {
                return false;
            }
            if(!room.isCompleted)
            {
                return false;
            }
        }
        return true;
    }
}