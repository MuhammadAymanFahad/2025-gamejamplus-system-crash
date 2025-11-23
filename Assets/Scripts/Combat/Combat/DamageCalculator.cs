using UnityEngine;

public static class DamageCalculator
{
    // Player attack damage
    public static int CalculatePlayerDamage(int weaponGrade)
    {
        return weaponGrade > 0 ? weaponGrade : 1;
    }

    // ✅ Monster attack damage - reduced by weapon grade!
    public static int CalculateMonsterDamage(CardTest monster, int playerWeaponGrade)
    {
        int baseDamage = monster.grade;
        int reducedDamage = baseDamage - playerWeaponGrade;
        return Mathf.Max(reducedDamage, 1); // Minimum 1 damage
    }

    // ✅ Use this to calculate remaining HP (always >= 0)
    public static int CalculateRemainingHP(int currentHP, int damage)
    {
        int result = currentHP - damage;
        return Mathf.Max(0, result); // ✅ Never goes below 0
    }

    public static bool IsLethal(int currentHP, int damage)
    {
        return currentHP - damage <= 0;
    }
}