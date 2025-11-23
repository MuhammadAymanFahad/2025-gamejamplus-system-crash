using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour, ICombatSystem
{
    public static CombatManager Instance { get; private set; }

    private TurnManager turnManager;
    private PlayerManager player;
    private bool inCombat = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            turnManager = GetComponent<TurnManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = PlayerManager.Instance;

        CombatEvents.OnCombatEnd += HandleCombatEnd;
        CombatEvents.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        // ✅ Unsubscribe to prevent memory leaks
        CombatEvents.OnCombatEnd -= HandleCombatEnd;
        CombatEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    void HandleCombatEnd()
    {
        if (!inCombat) return;

        Debug.Log("=== COMBAT END - VICTORY ===");
        EndCombat();
    }

    void HandlePlayerDeath()
    {
        if (!inCombat) return;

        Debug.Log("=== COMBAT END - DEFEAT ===");
        EndCombat();
        // TODO: Trigger Game Over screen
    }

    public void StartCombat(List<CardTest> roomCards)
    {
        List<CardTest> monsters = roomCards.FindAll(c => c.isMonster);

        if (monsters.Count == 0)
        {
            Debug.Log("No monsters in room!");
            return;
        }

        inCombat = true;
        player.SetCombatState(true); // ✅ Use interface method

        turnManager.InitializeCombat(monsters);

        Debug.Log($"=== COMBAT START === {monsters.Count} monsters");
    }

    public void PlayerAttack(CardTest targetMonster)
    {
        if (!inCombat) return;
        if (!turnManager.IsPlayerTurn())
        {
            Debug.Log("Not player's turn!");
            return;
        }

        turnManager.PlayerAttackMonster(targetMonster);
    }

    // ✅ NEW: Public method untuk heal (consumes turn)
    public void PlayerHeal(int potency)
    {
        if (!inCombat)
        {
            // Heal outside combat (no turn consumed)
            player.Heal(potency);
            return;
        }

        if (!turnManager.IsPlayerTurn())
        {
            Debug.Log("Not player's turn!");
            return;
        }

        // Heal in combat (consumes turn)
        turnManager.PlayerUsePotion(potency);
    }

    public void EndCombat()
    {
        inCombat = false;
        player.SetCombatState(false); // ✅ Use method instead of direct access
        Debug.Log("Combat system cleaned up!");
    }

    public bool IsInCombat() => inCombat;
}