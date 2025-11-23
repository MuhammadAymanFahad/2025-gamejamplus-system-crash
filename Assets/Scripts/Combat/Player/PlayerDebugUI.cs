using UnityEngine;

// ✅ Separate debug UI - can be disabled for production
public class PlayerDebugUI : MonoBehaviour
{
    private PlayerStats stats;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;

        GUI.Label(new Rect(10, 10, 300, 30), $"HP: {stats.currentHP}/{stats.maxHP}", style);
        GUI.Label(new Rect(10, 40, 300, 30), $"Weapon: Grade {stats.weaponGrade} ({stats.weaponKillCount}/5 kills)", style);
        GUI.Label(new Rect(10, 70, 300, 30), $"Can Flee: {stats.canFlee}", style);
        GUI.Label(new Rect(10, 100, 300, 30), $"In Combat: {stats.isInCombat}", style);
    }
}