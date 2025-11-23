using UnityEngine;

public static class DamageCalculator
{
    // Player attack damage
    public static int CalculatePlayerDamage(int weaponGrade)
    {
        return weaponGrade > 0 ? weaponGrade : 1; // barehanded = 1
    }

    // ✅ Monster attack damage - reduced by weapon grade!
    public static int CalculateMonsterDamage(CardTest monster, int playerWeaponGrade)
    {
        int baseDamage = monster.grade;
        int reducedDamage = baseDamage - playerWeaponGrade;

        // Minimum damage is 1 (cannot be 0 or negative)
        return Mathf.Max(reducedDamage, 1);
    }

    public static int CalculateRemainingHP(int currentHP, int damage)
    {
        int result = currentHP - damage;
        return Mathf.Max(0, result);
    }

    public static bool IsLethal(int currentHP, int damage)
    {
        return currentHP - damage <= 0;
    }
}