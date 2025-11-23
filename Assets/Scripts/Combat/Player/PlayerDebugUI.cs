using UnityEngine;

// ✅ Separate debug UI - can be disabled for production
public class PlayerDebugUI : MonoBehaviour
{
    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }
}