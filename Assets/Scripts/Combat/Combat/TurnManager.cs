using System.Collections.Generic;
using UnityEngine;

public enum TurnPhase
{
    PlayerTurn,
    MonsterTurn,
    CombatEnd
}

public class TurnManager : MonoBehaviour
{
    public TurnPhase currentPhase = TurnPhase.PlayerTurn;

    private List<CardTest> activeMonsters = new List<CardTest>();
    private PlayerManager player;

    void Start()
    {
        player = PlayerManager.Instance;
    }

    public void InitializeCombat(List<CardTest> monsters)
    {
        activeMonsters = new List<CardTest>(monsters);

        // ✅ INITIALIZE HP untuk setiap monster
        foreach (CardTest monster in activeMonsters)
        {
            monster.InitializeCombat(); // currentHP = grade
        }

        currentPhase = TurnPhase.PlayerTurn;
        Debug.Log($"Combat started! {activeMonsters.Count} monsters");

        // Debug info
        foreach (CardTest m in activeMonsters)
        {
            Debug.Log($"  - Monster grade {m.grade}, HP: {m.currentHP}");
        }
    }

    public void PlayerAttackMonster(CardTest targetMonster)
    {
        if (currentPhase != TurnPhase.PlayerTurn)
        {
            Debug.Log("Not player turn!");
            return;
        }

        // ✅ Player attacks - KURANGI currentHP, bukan grade!
        int damage = DamageCalculator.CalculatePlayerDamage(player.WeaponGrade);
        targetMonster.currentHP -= damage;

        Debug.Log($"Player attacks for {damage}! Monster HP: {targetMonster.currentHP}/{targetMonster.grade}");

        // Check if monster died
        if (targetMonster.currentHP <= 0)
        {
            OnMonsterKilled(targetMonster);
        }

        // End player turn
        EndPlayerTurn();
    }

    public void PlayerUsePotion(int potency)
    {
        if (currentPhase != TurnPhase.PlayerTurn) return;

        player.Heal(potency);
        Debug.Log($"Player used potion (grade {potency})");

        // Potion consumes turn
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        currentPhase = TurnPhase.MonsterTurn;
        MonstersTurn();
    }

    void MonstersTurn()
    {
        Debug.Log("--- MONSTERS TURN ---");

        foreach (CardTest monster in activeMonsters)
        {
            // ✅ Pass player weapon grade untuk damage reduction
            int damage = DamageCalculator.CalculateMonsterDamage(monster, player.WeaponGrade);

            // Show damage calculation
            int baseDamage = monster.grade;
            int reduction = player.WeaponGrade;
            Debug.Log($"Monster (grade {baseDamage}) attacks! Damage: {baseDamage} - {reduction} = {damage}");

            player.TakeDamage(damage);

            if (!player.IsAlive)
            {
                currentPhase = TurnPhase.CombatEnd;
                return;
            }
        }

        // Check if combat should end
        if (activeMonsters.Count == 0)
        {
            currentPhase = TurnPhase.CombatEnd;
            EndCombat();
        }
        else
        {
            currentPhase = TurnPhase.PlayerTurn;
            Debug.Log("--- PLAYER TURN ---");
        }
    }

    void OnMonsterKilled(CardTest monster)
    {
        Debug.Log($"Monster KILLED! Grade: {monster.grade}, Final HP: {monster.currentHP}");

        activeMonsters.Remove(monster);
        CombatEvents.TriggerMonsterKilled(monster);

        // ✅ Weapon adopts GRADE (bukan currentHP!)
        if (player.WeaponGrade > 0 && !monster.isBoss)
        {
            player.AdoptWeaponGrade(monster.grade); // ✅ Pakai grade, bukan currentHP!
        }

        // Boss special
        if (monster.isBoss)
        {
            CombatEvents.TriggerBossKilled(monster);
        }

        // TODO: Send to discard pile
    }

    void EndCombat()
    {
        Debug.Log("=== COMBAT END ===");
        CombatEvents.TriggerCombatEnd();
    }

    // Getters
    public bool IsPlayerTurn() => currentPhase == TurnPhase.PlayerTurn;
    public List<CardTest> GetActiveMonsters() => activeMonsters;
}