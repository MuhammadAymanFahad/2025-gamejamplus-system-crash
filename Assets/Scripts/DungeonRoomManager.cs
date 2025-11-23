using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class DungeonRoomManager : MonoBehaviour
{
    public static DungeonRoomManager Instance { get; private set; }
    
    [Header("Level Configuration")]
    [Tooltip("Number of regular rooms (Combat/Reward) to generate")]
    public int totalRegularRooms = 7;
    public int currentLevelIndex = 1;
    
    [Tooltip("Number of boss rooms to generate (typically 1)")]
    public int bossRoom = 1;
    public bool isHaveKey = false;

    [Header("Scene References")]
    [Tooltip("Name of the map/exploration scene")]
    [SerializeField] private string mapSceneName ;
    
    [Tooltip("Name of the combat encounter scene")]
    public string combatSceneName = "CombatScene";
    
    [Tooltip("Name of the boss battle scene")]
    public string bossSceneName = "BossScene";

    public string nextFloorSceneName = "World_2";
    public string mainMenuSceneName = "01_MainMenu";
    
    private List<RoomType> dungeonRooms = new List<RoomType>();
    
    private int currentRoomIndex = 0;

    /// Initializes singleton instance and prevents destruction on scene load
    void Awake()
    {
        // Implement singleton pattern
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
    
    /// Generates the dungeon layout on game start
    void Start()
    {
        GenerateDungeonRooms();
    }
    
    
    /// Generates a randomized sequence of rooms for the dungeon
    /// Structure: Entrance → Regular Rooms (Combat/Reward) → Boss Room(s)
    private void GenerateDungeonRooms()
    {
        dungeonRooms.Clear();
        
        // Always start with entrance room
        dungeonRooms.Add(RoomType.Entrance);

        // Generate regular rooms based on weighted probabilities
        for (int i = 0; i < totalRegularRooms; i++)
        {
            dungeonRooms.Add(RoomType.Combat);
        }

        // Add boss room(s) at the end
        for (int i = 0; i < bossRoom; i++)
        {
            dungeonRooms.Add(RoomType.Boss);
        }


        // Initialize progression at entrance
        currentRoomIndex = 0;
    }


   
    
    
    /// Advances to the next room in the dungeon sequence
    /// Called when player enters a RoomExit trigger in the map scene
    /// Flow:
    /// 1. Increment room index
    /// 2. Check if dungeon is completed
    /// 3. Load appropriate scene for next room type
    public void PlayerEnteredRoom()
    {
        // Move to next room
        currentRoomIndex++;
    
        // Check for dungeon completion
        if (currentRoomIndex >= dungeonRooms.Count)
        {
            currentRoomIndex = dungeonRooms.Count - 1;
            return;
        }
        
        // Get next room type and load its scene
        RoomType nextRoom = dungeonRooms[currentRoomIndex];
        LoadRoomScene(nextRoom);

        
    }

    /// Loads the appropriate scene based on room type
    private void LoadRoomScene(RoomType roomType)
    {
        string sceneToLoad = GetSceneNameForRoomType(roomType);
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    /// Maps room types to their corresponding scene names
    private string GetSceneNameForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Combat:
                return combatSceneName;
            case RoomType.Boss:
                return bossSceneName;
            case RoomType.Entrance:
                // Entrance doesn't need scene transition
                return string.Empty;
            default:
                Debug.LogWarning($"[DungeonRoomManager] Unknown room type: {roomType}");
                return string.Empty;
        }
    }
    
    /// Returns player to the map scene after completing a room
    /// Call this from combat/reward scene managers when room is cleared
    public void ReturnToMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }
    
    /// Called when player has completed all rooms in the dungeon
    /// Override or extend this method to implement victory/ending logic
    public void OnDungeonComplete()
    {
        Debug.Log("[DungeonRoomManager] Dungeon completed!");

        if (isHaveKey)
        {
            if(currentLevelIndex >= 2)
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                currentLevelIndex++;
                SceneManager.LoadScene(nextFloorSceneName);
            }
            
        }
        
    }
    
    
    /// Gets the current room index in the dungeon progression
    public int GetCurrentRoomIndex()
    {
        return currentRoomIndex;
    }
    
    /// Gets the total number of rooms in the dungeon
    public int GetTotalRooms()
    {
        return dungeonRooms.Count;
    }
    

    /// Gets the type of the current room
    public RoomType GetCurrentRoomType()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < dungeonRooms.Count)
        {
            return dungeonRooms[currentRoomIndex];
        }
        return RoomType.Empty;
    }
    
    /// Gets the complete dungeon layout for UI display or debugging
    public IReadOnlyList<RoomType> GetDungeonLayout()
    {
        return dungeonRooms.AsReadOnly();
    }

    public void InitializeNewFloor()
    {
        isHaveKey = false;
        currentRoomIndex = 0;
        GenerateDungeonRooms();
        
    }
    

}