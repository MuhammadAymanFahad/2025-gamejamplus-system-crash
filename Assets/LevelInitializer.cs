using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    void Start()
    {
        // ❌ DISABLED - Single floor game, no need to initialize new floor
        // DungeonRoomManager already initializes in its own Start()

        /*
        if (DungeonRoomManager.Instance != null)
        {
            DungeonRoomManager.Instance.InitializeNewFloor();
        }
        */

        Debug.Log("[LevelInitializer] Disabled - Single floor mode");
    }
}