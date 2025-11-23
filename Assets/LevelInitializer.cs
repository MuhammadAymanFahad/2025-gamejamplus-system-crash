using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DungeonRoomManager.Instance != null)
        {
            DungeonRoomManager.Instance.InitializeNewFloor();
        }
        else
        {
            Debug.LogError("DungeonRoomManager instance not found in the scene.");
        }
    }
}
