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

    private List<CombatCard> activeMonsters = new List<CombatCard>();
    private PlayerManager player;

    void Start()
    {
        player = PlayerManager.Instance;
    }

    public void InitializeCombat(List<CombatCard> monsters)
    {
        activeMonsters = new List<CombatCard>(monsters);

        // ✅ INITIALIZE HP untuk setiap monster
        foreach (CombatCard monster in activeMonsters)
        {
            monster.InitializeCombat(); // currentHP = grade
        }

        currentPhase = TurnPhase.PlayerTurn;
        Debug.Log($"Combat started! {activeMonsters.Count} monsters");

        // Debug info
        foreach (CombatCard m in activeMonsters)
        {
            Debug.Log($"  - Monster grade {m.grade}, HP: {m.currentHP}");
        }
    }

    public void PlayerAttackMonster(CombatCard targetMonster)
    {
        if (currentPhase != TurnPhase.PlayerTurn)
        {
            Debug.Log("Not player turn!");
            return;
        }

        // Player attacks
        int damage = DamageCalculator.CalculatePlayerDamage(player.WeaponGrade);

        // ✅ Use DamageCalculator utility
        targetMonster.currentHP = DamageCalculator.CalculateRemainingHP(targetMonster.currentHP, damage);

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
        if (currentPhase != TurnPhase.PlayerTurn)
        {
            Debug.Log("Not player turn!");
            return;
        }

        player.Heal(potency);
        Debug.Log($"Player used potion (grade {potency}) - Turn consumed!");

        // ✅ Potion consumes turn - monsters attack now!
        EndPlayerTurn();
    }

    void EndPlayerTurn()
    {
        // ✅ Check if combat should end BEFORE monster turn
        if (activeMonsters.Count == 0)
        {
            currentPhase = TurnPhase.CombatEnd;
            CombatEvents.TriggerCombatEnd();
            return; // ✅ Don't proceed to monster turn!
        }

        currentPhase = TurnPhase.MonsterTurn;
        MonstersTurn();
    }

    void MonstersTurn()
    {
        Debug.Log("--- MONSTERS TURN ---");

        foreach (CombatCard monster in activeMonsters)
        {
            int damage = DamageCalculator.CalculateMonsterDamage(monster, player.WeaponGrade);
            int baseDamage = monster.grade;
            int reduction = player.WeaponGrade;
            Debug.Log($"Monster (grade {baseDamage}) attacks! Damage: {baseDamage} - {reduction} = {damage}");

            player.TakeDamage(damage);

            if (!player.IsAlive)
            {
                currentPhase = TurnPhase.CombatEnd;
                CombatEvents.TriggerPlayerDeath();
                return;
            }
        }

        // ✅ Double-check after attacks (in case monster died from counter-attack or something)
        if (activeMonsters.Count == 0)
        {
            currentPhase = TurnPhase.CombatEnd;
            CombatEvents.TriggerCombatEnd();
        }
        else
        {
            currentPhase = TurnPhase.PlayerTurn;
            Debug.Log("--- PLAYER TURN ---");
        }
    }

    void OnMonsterKilled(CombatCard monster)
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
    public List<CombatCard> GetActiveMonsters() => activeMonsters;
}