using System.Collections.Generic;
using UnityEngine;

// ✅ Separate test script - can be removed for production
public class CombatTester : MonoBehaviour
{
    private CombatManager combatManager;
    private PlayerManager player;

    void Start()
    {
        combatManager = CombatManager.Instance;
        player = PlayerManager.Instance;
    }

    void Update()
    {
        // Start test combat
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestCombat();
        }

        // Test weapon equip
        if (Input.GetKeyDown(KeyCode.W))
        {
            player.EquipWeapon(6);
        }

        // In-combat actions
        if (combatManager.IsInCombat())
        {
            var turnManager = combatManager.GetComponent<TurnManager>();

            if (turnManager.IsPlayerTurn())
            {
                // Attack
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    var monsters = turnManager.GetActiveMonsters();
                    if (monsters.Count > 0)
                    {
                        combatManager.PlayerAttack(monsters[0]);
                    }
                }

                // Heal
                if (Input.GetKeyDown(KeyCode.H))
                {
                    combatManager.PlayerHeal(6);
                }
            }
        }
    }

    void TestCombat()
    {
        List<Card> testCards = new List<Card> {
            new Card("Clover", 4),
            new Card("Spade", 10),
            new Card("Heart", 5),
            new Card("Diamond", 6)
        };

        combatManager.StartCombat(testCards);
    }
}
