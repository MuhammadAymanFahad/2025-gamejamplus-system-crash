using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

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
    }

    // ICombatSystem implementation
    public void StartCombat(List<Card> roomCards)
    {
        // Filter hanya monster
        List<Card> monsters = roomCards.FindAll(c => c.isMonster);

        if (monsters.Count == 0)
        {
            Debug.Log("No monsters in room!");
            return;
        }

        inCombat = true;
        player.GetComponent<PlayerStats>().isInCombat = true;

        turnManager.InitializeCombat(monsters);

        Debug.Log($"=== COMBAT START === {monsters.Count} monsters");
    }

    public void PlayerAttack(Card targetMonster)
    {
        if (!inCombat) return;
        turnManager.PlayerAttackMonster(targetMonster);
    }

    public void PlayerUsePotion(int potency)
    {
        if (!inCombat) return;
        turnManager.PlayerUsePotion(potency);
    }

    public void EndCombat()
    {
        inCombat = false;
        player.GetComponent<PlayerStats>().isInCombat = false;
        Debug.Log("Combat ended!");
    }

    public bool IsInCombat() => inCombat;

    // Testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestCombat();
        }

        if (Input.GetKeyDown(KeyCode.Space) && inCombat)
        {
            // Attack first monster
            var monsters = turnManager.GetActiveMonsters();
            if (monsters.Count > 0)
            {
                PlayerAttack(monsters[0]);
            }
        }
    }

    void TestCombat()
    {
        List<Card> testCards = new List<Card> {
        new Card("Clover", 4),   // ✅ Monster grade 4, HP 4
        new Card("Spade", 10),   // ✅ Monster grade 10, HP 10
        new Card("Heart", 5),    // Potion
        new Card("Diamond", 6)   // Weapon
    };

        StartCombat(testCards);
    }
}