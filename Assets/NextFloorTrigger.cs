using UnityEngine;

public class BossDefeatedTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if boss is defeated (implement your own logic)
            bool bossDefeated = CheckBossDefeated();

            if (bossDefeated)
            {
                // End game - return to main menu
                DungeonRoomManager.Instance.OnDungeonComplete();
            }
            else
            {
                Debug.Log("Defeat the boss first!");
            }
        }
    }

    bool CheckBossDefeated()
    {
        // TODO: Implement boss check logic
        // For now, return true for testing
        return true;
    }
}