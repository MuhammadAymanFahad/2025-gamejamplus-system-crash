using System;
using System.Collections.Generic;

// Temporary Card class (nanti diganti punya Programmer 1)
    public class CardTest 
    {
        public string suit; // "Heart", "Diamond", "Clover", "Spade"
        public int grade;   // 2-10, or 15 for boss (IMMUTABLE!)

        // ✅ TAMBAHKAN INI - Runtime data
        public int currentHP; // Current HP untuk monster (berubah saat combat)

        public bool isMonster => suit == "Clover" || suit == "Spade";
        public bool isBoss => grade == 15;

        // Constructor helper
        public CardTest(string suit, int grade)
        {
            this.suit = suit;
            this.grade = grade;
            this.currentHP = grade; // ✅ HP awal = grade
        }

        // Empty constructor (untuk compatibility)
        public CardTest()
        {
            currentHP = grade;
        }

        // Method untuk init HP saat masuk combat
        public void InitializeCombat()
        {
            currentHP = grade; // Reset HP = grade
        }
    }

// Interface untuk Combat System
public interface ICombatSystem
{
    void StartCombat(List<CardTest> roomCards);
    void PlayerAttack(CardTest targetMonster);
    void EndCombat();
    bool IsInCombat();
}

// Interface untuk Player
public interface IPlayer
{
    int CurrentHP { get; }
    int MaxHP { get; }
    int WeaponGrade { get; }
    bool CanFlee { get; }
    void TakeDamage(int damage);
    void Heal(int amount);
    void EquipWeapon(int grade);
}

// Events untuk komunikasi antar system
public static class CombatEvents
{
    public static event Action<CardTest> OnMonsterKilled;
    public static event Action OnPlayerDeath;
    public static event Action OnCombatEnd;
    public static event Action<CardTest> OnBossKilled;

    public static void TriggerMonsterKilled(CardTest monster) => OnMonsterKilled?.Invoke(monster);
    public static void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();
    public static void TriggerCombatEnd() => OnCombatEnd?.Invoke();
    public static void TriggerBossKilled(CardTest boss) => OnBossKilled?.Invoke(boss);
}