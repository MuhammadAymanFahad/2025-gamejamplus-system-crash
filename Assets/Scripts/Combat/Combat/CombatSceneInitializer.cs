using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Initializes combat scene with room card data from DungeonRoomManager
/// Attach this to an empty GameObject in your Combat scene (Battle scene)
/// </summary>
public class CombatSceneInitializer : MonoBehaviour
{
    void Start()
    {
        InitializeCombat();
    }

    void InitializeCombat()
    {
        Debug.Log("[CombatInit] Starting combat initialization...");

        // Get current room data from DungeonRoomManager
        if (DungeonRoomManager.Instance == null)
        {
            Debug.LogError("[CombatInit] DungeonRoomManager.Instance is NULL! Make sure DungeonRoomManager exists and is DontDestroyOnLoad.");
            return;
        }

        FloorRoomData roomData = DungeonRoomManager.Instance.GetCurrentRoomData();

        if (roomData == null)
        {
            Debug.LogError("[CombatInit] No room data found! Make sure rooms are generated before entering combat.");
            return;
        }

        if (roomData.roomCards == null || roomData.roomCards.Count == 0)
        {
            Debug.LogError("[CombatInit] Room has no cards!");
            return;
        }

        Debug.Log($"[CombatInit] Initializing with {roomData.roomCards.Count} cards from room {roomData.roomIndex}");

        // Log all cards in room
        foreach (var card in roomData.roomCards)
        {
            Debug.Log($"  - {card.suit} {card.rank} (Grade {card.Grade}, Monster: {card.IsMonster}, Potion: {card.IsPotion}, Weapon: {card.IsWeapon})");
        }

        // Convert RoomCardData to CombatCard format
        List<CombatCard> combatCards = ConvertToCombatCards(roomData.roomCards);

        // Start combat with converted cards
        if (CombatManager.Instance == null)
        {
            Debug.LogError("[CombatInit] CombatManager.Instance is NULL!");
            return;
        }

        Debug.Log($"[CombatInit] Calling CombatManager.StartCombat with {combatCards.Count} cards...");
        CombatManager.Instance.StartCombat(combatCards);
    }

    /// <summary>
    /// Converts RoomCardData (room cards) to CombatCard format (combat system)
    /// </summary>
    List<CombatCard> ConvertToCombatCards(List<RoomCardData> roomCards)
    {
        List<CombatCard> combatCards = new List<CombatCard>();

        foreach (RoomCardData data in roomCards)
        {
            // Convert CardSuit enum to string for CombatCard
            string suitString = ConvertSuitToString(data.suit);

            // Use constructor to create CombatCard
            CombatCard card = new CombatCard(suitString, data.Grade);
            card.InitializeCombat(); // Initialize HP

            combatCards.Add(card);

            Debug.Log($"[CombatInit] Converted: {data.suit} {data.rank} → CombatCard (Suit: {card.suit}, Grade: {card.grade}, HP: {card.currentHP}, Monster: {card.isMonster})");
        }

        return combatCards;
    }

    /// <summary>
    /// Converts CardSuit enum to string format expected by CombatCard
    /// </summary>
    string ConvertSuitToString(CardSuit suit)
    {
        switch (suit)
        {
            case CardSuit.Heart:
                return "Heart";
            case CardSuit.Diamond:
                return "Diamond";
            case CardSuit.Clover:
                return "Clover";
            case CardSuit.Spade:
                return "Spade"; // Note: Shovel → Spade
            default:
                Debug.LogWarning($"Unknown suit: {suit}, defaulting to Heart");
                return "Heart";
        }
    }
}