using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DungeonRoomManager : MonoBehaviour
{
    public static DungeonRoomManager Instance;

    [Header("Level Config")]
    public int totalRegularRooms = 10;
    public int bossRoom = 1;

    [Header("Room Weights")]
    [Range(0, 100)] public int combatWeight = 70;
    [Range(0, 100)] public int rewardWeight = 30;

    [Header("Scene")]
    public string combatSceneName = "CombatScene";
    public string rewardSceneName = "RewardScene";
    public string bossSceneName = "BossScene";

    private List<RoomType> dungeonRooms = new List<RoomType>();
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
    }

    private void GenerateDungeonRooms()
    {
        dungeonRooms.Clear();
        dungeonRooms.Add(RoomType.Entrance);

        for (int i = 0; i < totalRegularRooms; i++)
        {
            dungeonRooms.Add(GetRandomRoomType());
        }

        for (int i = 0; i < bossRoom; i++)
        {
            dungeonRooms.Add(RoomType.Boss);
        }

    }

    private RoomType GetRandomRoomType()
    {
        int totalWeight = combatWeight + rewardWeight;
        int randomValue = Random.Range(0, totalWeight);

        if (randomValue < combatWeight)
            return RoomType.Combat;
        else
            return RoomType.Reward;
    }

    public void PlayerEnteredRoom()
    {
        RoomType nextRoom = dungeonRooms[currentRoomIndex];
        LoadRoomScene(nextRoom);
    }

    private void LoadRoomScene(RoomType roomType)
    {
        string sceneToLoad = "";

        switch (roomType)
        {   
            case RoomType.Combat:
                UnityEngine.SceneManagement.SceneManager.LoadScene(combatSceneName);
                break;
            case RoomType.Reward:
                UnityEngine.SceneManagement.SceneManager.LoadScene(rewardSceneName);
                break;
            case RoomType.Boss:
                UnityEngine.SceneManagement.SceneManager.LoadScene(bossSceneName);
                break;
            case RoomType.Entrance:
                Debug.Log("DungeonRoomManager: Player entered the Entrance room.");
                break;
            default:
                Debug.LogWarning("DungeonRoomManager: Unknown room type!");
                break;
        }
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}
