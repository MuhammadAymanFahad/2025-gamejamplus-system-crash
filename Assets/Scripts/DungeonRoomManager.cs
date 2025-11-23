using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages dungeon progression and room generation for a roguelike game.
/// Handles scene transitions between map exploration and combat/reward rooms.
/// 
/// Features:
/// - Procedural room generation with weighted probabilities
/// - Singleton pattern for persistence across scenes
/// - Progress tracking and scene management
/// 
/// Setup Requirements:
/// - Attach to a GameObject in the initial scene
/// - Configure scene names in Inspector to match your scene assets
/// - Add all scenes to Build Settings
/// 
/// Usage Flow:
/// 1. Player explores map scene
/// 2. Player enters RoomExit trigger
/// 3. PlayerEnteredRoom() is called
/// 4. Next room scene is loaded based on generated dungeon
/// 5. After room completion, call ReturnToMap()
/// </summary>
public class DungeonRoomManager : MonoBehaviour
{
    #region Singleton
    
    /// <summary>
    /// Singleton instance accessible from any scene
    /// </summary>
    public static DungeonRoomManager Instance { get; private set; }
    
    #endregion
    
    #region Inspector Fields
    
    [Header("Level Configuration")]
    [Tooltip("Number of regular rooms (Combat/Reward) to generate")]
    public int totalRegularRooms = 10;
    
    [Tooltip("Number of boss rooms to generate (typically 1)")]
    public int bossRoom = 1;

    [Header("Room Generation Weights")]
    [Tooltip("Probability weight for combat rooms (0-100)")]
    [Range(0, 100)] public int combatWeight = 70;
    
    [Tooltip("Probability weight for reward rooms (0-100)")]
    [Range(0, 100)] public int rewardWeight = 30;

    [Header("Scene References")]
    [Tooltip("Name of the map/exploration scene")]
    [SerializeField] private string mapSceneName ;
    
    [Tooltip("Name of the combat encounter scene")]
    public string combatSceneName = "CombatScene";
    
    [Tooltip("Name of the reward/treasure scene")]
    public string rewardSceneName = "RewardScene";
    
    [Tooltip("Name of the boss battle scene")]
    public string bossSceneName = "BossScene";
    
    #endregion
    
    #region Private Fields
    
    /// <summary>
    /// Ordered list of room types representing the dungeon layout
    /// </summary>
    private List<RoomType> dungeonRooms = new List<RoomType>();
    
    /// <summary>
    /// Current position in the dungeon progression (0-indexed)
    /// </summary>
    private int currentRoomIndex = 0;
    
    #endregion
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initializes singleton instance and prevents destruction on scene load
    /// </summary>
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
    
    /// <summary>
    /// Generates the dungeon layout on game start
    /// </summary>
    void Start()
    {
        GenerateDungeonRooms();
    }
    
    #endregion
    
    #region Dungeon Generation
    
    /// <summary>
    /// Generates a randomized sequence of rooms for the dungeon
    /// Structure: Entrance → Regular Rooms (Combat/Reward) → Boss Room(s)
    /// </summary>
    private void GenerateDungeonRooms()
    {
        dungeonRooms.Clear();
        
        // Always start with entrance room
        dungeonRooms.Add(RoomType.Entrance);

        // Generate regular rooms based on weighted probabilities
        for (int i = 0; i < totalRegularRooms; i++)
        {
            dungeonRooms.Add(GetRandomRoomType());
        }

        // Add boss room(s) at the end
        for (int i = 0; i < bossRoom; i++)
        {
            dungeonRooms.Add(RoomType.Boss);
        }

        // Initialize progression at entrance
        currentRoomIndex = 0;
    }

    /// <summary>
    /// Selects a random room type based on configured weight probabilities
    /// Higher weight = higher chance of selection
    /// </summary>
    /// <returns>RoomType.Combat or RoomType.Reward</returns>
    private RoomType GetRandomRoomType()
    {
        int totalWeight = combatWeight + rewardWeight;
        int randomValue = Random.Range(0, totalWeight);

        // Check if random value falls within combat weight range
        if (randomValue < combatWeight)
            return RoomType.Combat;
        else
            return RoomType.Reward;
    }
    
    #endregion
    
    #region Room Progression
    
    /// <summary>
    /// Advances to the next room in the dungeon sequence
    /// Called when player enters a RoomExit trigger in the map scene
    /// 
    /// Flow:
    /// 1. Increment room index
    /// 2. Check if dungeon is completed
    /// 3. Load appropriate scene for next room type
    /// </summary>
    public void PlayerEnteredRoom()
    {
        // Move to next room
        currentRoomIndex++;
        
        // Check for dungeon completion
        if (currentRoomIndex >= dungeonRooms.Count)
        {
            OnDungeonComplete();
            return;
        }
        
        // Get next room type and load its scene
        RoomType nextRoom = dungeonRooms[currentRoomIndex];
        LoadRoomScene(nextRoom);
    }

    /// <summary>
    /// Loads the appropriate scene based on room type
    /// </summary>
    /// <param name="roomType">Type of room to load</param>
    private void LoadRoomScene(RoomType roomType)
    {
        string sceneToLoad = GetSceneNameForRoomType(roomType);
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    /// <summary>
    /// Maps room types to their corresponding scene names
    /// </summary>
    /// <param name="roomType">The type of room</param>
    /// <returns>Scene name string, or empty if invalid room type</returns>
    private string GetSceneNameForRoomType(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Combat:
                return combatSceneName;
            case RoomType.Reward:
                return rewardSceneName;
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
    
    /// <summary>
    /// Returns player to the map scene after completing a room
    /// Call this from combat/reward scene managers when room is cleared
    /// </summary>
    public void ReturnToMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }
    
    /// <summary>
    /// Called when player has completed all rooms in the dungeon
    /// Override or extend this method to implement victory/ending logic
    /// </summary>
    private void OnDungeonComplete()
    {
        Debug.Log("[DungeonRoomManager] Dungeon completed!");
        
        // TODO: Implement ending sequence
        // Examples:
        // - Load victory scene
        // - Show completion stats
        // - Return to main menu
        // - Start next level/floor
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Gets the current room index in the dungeon progression
    /// </summary>
    /// <returns>Zero-based index of current room</returns>
    public int GetCurrentRoomIndex()
    {
        return currentRoomIndex;
    }
    
    /// <summary>
    /// Gets the total number of rooms in the dungeon
    /// </summary>
    /// <returns>Total room count including entrance and boss</returns>
    public int GetTotalRooms()
    {
        return dungeonRooms.Count;
    }
    
    /// <summary>
    /// Gets the type of the current room
    /// </summary>
    /// <returns>RoomType enum value, or RoomType.Empty if index out of bounds</returns>
    public RoomType GetCurrentRoomType()
    {
        if (currentRoomIndex >= 0 && currentRoomIndex < dungeonRooms.Count)
        {
            return dungeonRooms[currentRoomIndex];
        }
        return RoomType.Empty;
    }
    
    /// <summary>
    /// Gets the complete dungeon layout for UI display or debugging
    /// </summary>
    /// <returns>Read-only list of room types</returns>
    public IReadOnlyList<RoomType> GetDungeonLayout()
    {
        return dungeonRooms.AsReadOnly();
    }
    
    #endregion
}