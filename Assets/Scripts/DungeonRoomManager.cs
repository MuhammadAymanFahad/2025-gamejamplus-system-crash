using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DungeonRoomManager : MonoBehaviour
{
    public static DungeonRoomManager Instance { get; private set; }

    [Header("Level Configuration")]
    [Tooltip("Number of regular rooms (Combat/Reward) to generate")]
    public int totalRegularRooms = 12; // Single floor, more rooms

    [Tooltip("Number of boss rooms to generate (typically 1)")]
    public int bossRoom = 1;

    [Header("Scene References")]
    [Tooltip("Name of the map/exploration scene")]
    [SerializeField] private string mapSceneName;

    [Tooltip("Name of the combat encounter scene")]
    public string combatSceneName = "CombatScene";

    [Tooltip("Name of the boss battle scene")]
    public string bossSceneName = "BossScene";

    public string mainMenuSceneName = "01_MainMenu";

    private List<RoomType> dungeonRooms = new List<RoomType>();

    // ✅ TAMBAHKAN INI - Room content data
    private List<FloorRoomData> floorRoomContents = new List<FloorRoomData>();

    private int currentRoomIndex = 0;

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

    void Start()
    {
        GenerateDungeonRooms();

        // ✅ TAMBAHKAN INI - Initialize room deck and generate content
        if (RoomDeckManager.Instance != null)
        {
            RoomDeckManager.Instance.InitializeRoomDeck();
            GenerateFloorContent();
        }
        else
        {
            Debug.LogError("[DungeonRoomManager] RoomDeckManager not found!");
        }
    }

    private void GenerateDungeonRooms()
    {
        dungeonRooms.Clear();

        dungeonRooms.Add(RoomType.Entrance);

        for (int i = 0; i < totalRegularRooms; i++)
        {
            dungeonRooms.Add(RoomType.Combat);
        }

        for (int i = 0; i < bossRoom; i++)
        {
            dungeonRooms.Add(RoomType.Boss);
        }

        currentRoomIndex = 0;
    }

    // ✅ TAMBAHKAN METHOD BARU INI - Generate room card content
    private void GenerateFloorContent()
    {
        floorRoomContents.Clear();

        for (int i = 0; i < dungeonRooms.Count; i++)
        {
            RoomType roomType = dungeonRooms[i];

            // Entrance doesn't need cards
            if (roomType == RoomType.Entrance)
            {
                floorRoomContents.Add(new FloorRoomData
                {
                    roomIndex = i,
                    roomCards = new List<RoomCardData>()
                });
                continue;
            }

            // Draw 3 cards for this room
            FloorRoomData roomData = new FloorRoomData
            {
                roomIndex = i,
                roomCards = RoomDeckManager.Instance.DrawRoomCards(3)
            };
            floorRoomContents.Add(roomData);

            // Log for debugging
            string cardInfo = "";
            foreach (var card in roomData.roomCards)
            {
                cardInfo += $"{card.suit} {card.rank}, ";
            }
            Debug.Log($"[Floor] Room {i} ({roomType}): {cardInfo.TrimEnd(',', ' ')}");
        }
    }

    // ✅ TAMBAHKAN METHOD BARU INI - Get current room data
    public FloorRoomData GetCurrentRoomData()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < floorRoomContents.Count)
        {
            return floorRoomContents[currentRoomIndex];
        }

        Debug.LogWarning($"[DungeonRoomManager] Invalid room index: {currentRoomIndex}");
        return null;
    }

    // ✅ TAMBAHKAN METHOD BARU INI - Mark room as cleared
    public void MarkCurrentRoomCleared()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < floorRoomContents.Count)
        {
            floorRoomContents[currentRoomIndex].isCleared = true;
            Debug.Log($"[DungeonRoomManager] Room {currentRoomIndex} marked as cleared");
        }
    }

    public void PlayerEnteredRoom()
    {
        currentRoomIndex++;

        if (currentRoomIndex >= dungeonRooms.Count)
        {
            currentRoomIndex = dungeonRooms.Count - 1;
            return;
        }

        RoomType nextRoom = dungeonRooms[currentRoomIndex];
        LoadRoomScene(nextRoom);
    }

    private void LoadRoomScene(RoomType roomType)
    {
        string sceneToLoad = GetSceneNameForRoomType(roomType);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private string GetSceneNameForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Combat:
                return combatSceneName;
            case RoomType.Boss:
                return bossSceneName;
            case RoomType.Entrance:
                return string.Empty;
            default:
                Debug.LogWarning($"[DungeonRoomManager] Unknown room type: {roomType}");
                return string.Empty;
        }
    }

    public void ReturnToMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }

    public void OnDungeonComplete()
    {
        Debug.Log("[DungeonRoomManager] Dungeon completed! - Single Floor Victory");

        // Single floor game - return to main menu
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public int GetCurrentRoomIndex()
    {
        return currentRoomIndex;
    }

    public int GetTotalRooms()
    {
        return dungeonRooms.Count;
    }

    public RoomType GetCurrentRoomType()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < dungeonRooms.Count)
        {
            return dungeonRooms[currentRoomIndex];
        }
        return RoomType.Empty;
    }

    public IReadOnlyList<RoomType> GetDungeonLayout()
    {
        return dungeonRooms.AsReadOnly();
    }

    // ✅ COMPATIBILITY: Dummy members for old multi-floor scripts
    // These do nothing in single floor mode but prevent compile errors

    public bool isHaveKey
    {
        get { return false; }
        set { /* Single floor mode - ignore */ }
    }

    public void InitializeNewFloor()
    {
        Debug.LogWarning("[DungeonRoomManager] InitializeNewFloor() called but game is single floor mode. Ignoring.");
    }
}