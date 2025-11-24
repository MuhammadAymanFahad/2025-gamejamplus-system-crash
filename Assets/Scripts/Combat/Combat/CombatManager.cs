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

    public void StartCombat(List<CombatCard> roomCards)
    {
        Debug.Log($"[CombatManager] ===== StartCombat CALLED ===== Received {roomCards?.Count ?? 0} cards");

        if (roomCards == null || roomCards.Count == 0)
        {
            Debug.LogError("[CombatManager] No room cards!");
            return;
        }

        foreach (var card in roomCards)
        {
            Debug.Log($"  - {card.suit} {card.grade} (Monster: {card.isMonster})");
        }

        inCombat = true;
        player.SetCombatState(true);

        List<CombatCard> monsters = roomCards.FindAll(c => c.isMonster);
        if (monsters.Count > 0)
        {
            Debug.Log($"[CombatManager] Initializing turn manager with {monsters.Count} monsters");
            turnManager.InitializeCombat(monsters);
        }
        else
        {
            Debug.Log("[CombatManager] No monsters - treasure room!");
        }

        if (CombatUIManager.Instance != null)
        {
            Debug.Log($"[CombatManager] Showing ALL {roomCards.Count} cards in UI");
            CombatUIManager.Instance.ShowCombat(roomCards); // Pass ALL cards!
        }

        Debug.Log("[CombatManager] ===== COMBAT START COMPLETE =====");
    }

    public void PlayerAttack(CombatCard targetMonster)
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