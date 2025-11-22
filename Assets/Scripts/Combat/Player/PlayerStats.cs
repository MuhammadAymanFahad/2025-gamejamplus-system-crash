using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 20;
    public int currentHP;

    [Header("Weapon")]
    public int weaponGrade = 0; // 0 = barehanded
    public int weaponKillCount = 0;
    public const int MAX_WEAPON_KILLS = 5;

    [Header("State")]
    public bool canFlee = true;
    public bool isInCombat = false;

    void Start()
    {
        currentHP = maxHP;
    }

    // Getters
    public bool HasWeapon() => weaponGrade > 0;
    public int GetDamage() => weaponGrade > 0 ? weaponGrade : 1;
    public bool IsAlive() => currentHP > 0;
    public bool ShouldDropWeapon() => weaponKillCount >= MAX_WEAPON_KILLS;
}