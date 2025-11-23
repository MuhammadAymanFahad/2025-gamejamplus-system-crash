using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour, ICombatSystem
{
    public static CombatManager Instance { get; private set; }

    private TurnManager turnManager;
    private PlayerManager player;
    private CombatUIManager combatUI; // ✅ NEW
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
        combatUI = CombatUIManager.Instance; // ✅ NEW

        CombatEvents.OnCombatEnd += HandleCombatEnd;
        CombatEvents.OnPlayerDeath += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        CombatEvents.OnCombatEnd -= HandleCombatEnd;
        CombatEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    public void StartCombat(List<CombatCard> roomCards)
    {
        List<CombatCard> monsters = roomCards.FindAll(c => c.isMonster);

        if (monsters.Count == 0)
        {
            Debug.Log("No monsters in room!");
            return;
        }

        inCombat = true;
        player.SetCombatState(true);

        turnManager.InitializeCombat(monsters);

        // ✅ Show UI with monsters
        if (combatUI != null)
        {
            combatUI.ShowCombat(monsters);
        }

        Debug.Log($"=== COMBAT START === {monsters.Count} monsters");
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

    public void PlayerHeal(int potency)
    {
        if (!inCombat)
        {
            player.Heal(potency);
            return;
        }

        if (!turnManager.IsPlayerTurn())
        {
            Debug.Log("Not player's turn!");
            return;
        }

        turnManager.PlayerUsePotion(potency);
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
    }

    public void EndCombat()
    {
        inCombat = false;
        player.SetCombatState(false);
        Debug.Log("Combat system cleaned up!");
    }

    public bool IsInCombat() => inCombat;
}
