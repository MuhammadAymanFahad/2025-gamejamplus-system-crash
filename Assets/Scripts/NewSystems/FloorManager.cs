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
        deckManager.BuildAndPartitionDeck();
        currentFloorDeck = deckManager.GetFloorDeck(floorIndex);

        int cardIndex = 0;
        for (int r= 0; r < 4; r++ )
        {
            var four = currentFloorDeck.GetRange(cardIndex, 4);
            rooms[r] = new RoomState(four);
            cardIndex += 4;
        }

        escapeTryCount = 0;
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