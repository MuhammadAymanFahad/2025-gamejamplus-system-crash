using UnityEngine;

public class NextFloorTrigger : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)
   {
       if (other.CompareTag("Player"))
       {
           DungeonRoomManager manager  = DungeonRoomManager.Instance;

           if (manager != null)
            {
                Debug.Log(manager.isHaveKey);
                if (!manager.isHaveKey)
                {
                    Debug.Log("Player cannot proceed to next floor yet");
                    return;
                }
                if (manager.isHaveKey)
                {
                    Debug.Log("Player has the key, proceeding to next floor");
                    manager.OnDungeonComplete();
                }
            }
       }
   }
}
