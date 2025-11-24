using UnityEngine;
using System.Collections.Generic;

public class CombatSceneInitializer : MonoBehaviour
{
    void Start()
    {
        InitializeCombat();
    }

    void InitializeCombat()
    {
        Debug.Log("[CombatInit] Starting combat initialization...");

        if (DungeonRoomManager.Instance == null)
        {
            Debug.LogError("[CombatInit] DungeonRoomManager.Instance is NULL!");
            return;
        }

        FloorRoomData roomData = DungeonRoomManager.Instance.GetCurrentRoomData();

        if (roomData == null || roomData.roomCards == null)
        {
            Debug.LogError("[CombatInit] No room data!");
            return;
        }

        Debug.Log($"[CombatInit] Initializing with {roomData.roomCards.Count} cards");

        // Store original RoomCardData for sprites
        List<RoomCardData> originalCards = roomData.roomCards;

        // Convert to CombatCard
        List<CombatCard> combatCards = ConvertToCombatCards(originalCards);

        // ✅ Pass BOTH lists to CombatManager
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.StartCombat(combatCards, originalCards);
        }
    }

    List<CombatCard> ConvertToCombatCards(List<RoomCardData> roomCards)
    {
        List<CombatCard> combatCards = new List<CombatCard>();

        foreach (RoomCardData data in roomCards)
        {
            string suitString = ConvertSuitToString(data.suit);
            CombatCard card = new CombatCard(suitString, data.Grade);
            card.InitializeCombat();

            combatCards.Add(card);
            Debug.Log($"[CombatInit] Converted: {data.suit} {data.rank} → CombatCard");
        }

        return combatCards;
    }

    string ConvertSuitToString(CardSuit suit)
    {
        switch (suit)
        {
            case CardSuit.Heart: return "Heart";
            case CardSuit.Diamond: return "Diamond";
            case CardSuit.Clover: return "Clover";
            case CardSuit.Spade: return "Spade";
            default: return "Heart";
        }
    }
}