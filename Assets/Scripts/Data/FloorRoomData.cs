using System.Collections.Generic;

/// <summary>
/// Data structure for a single room in the dungeon
/// Contains the 3 cards that will appear in this room
/// </summary>
[System.Serializable]
public class FloorRoomData
{
    public int roomIndex;
    public List<RoomCardData> roomCards;
    public bool isCleared = false;

    // Constructor
    public FloorRoomData()
    {
        roomCards = new List<RoomCardData>();
    }

    // Helper to check if room has monsters
    public bool HasMonsters()
    {
        if (roomCards == null) return false;
        return roomCards.Exists(card => card.IsMonster);
    }

    // Helper to get only monster cards
    public List<RoomCardData> GetMonsters()
    {
        if (roomCards == null) return new List<RoomCardData>();
        return roomCards.FindAll(card => card.IsMonster);
    }

    // Helper to get only treasure cards (potions + weapons)
    public List<RoomCardData> GetTreasures()
    {
        if (roomCards == null) return new List<RoomCardData>();
        return roomCards.FindAll(card => card.IsPotion || card.IsWeapon);
    }

    // Debug info
    public string GetRoomInfo()
    {
        string info = $"Room {roomIndex}: ";
        if (roomCards == null || roomCards.Count == 0)
        {
            return info + "Empty";
        }

        foreach (var card in roomCards)
        {
            info += $"{card.suit} {card.rank}, ";
        }
        return info.TrimEnd(',', ' ');
    }
}