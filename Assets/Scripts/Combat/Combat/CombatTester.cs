using System.Collections.Generic;
using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [Header("Settings")]
    public bool autoStartCombat = true; 
    public float autoStartDelay = 0.5f; 

    private CombatManager combatManager;
    private PlayerManager player;

    void Start()
    {
        combatManager = CombatManager.Instance;
        player = PlayerManager.Instance;

        // ✅ Auto-start combat if enabled
        if (autoStartCombat)
        {
            Invoke(nameof(TestCombat), autoStartDelay);
        }
    }

    void Update()
    {
        // Manual trigger (still available)
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestCombat();
        }

        // Test weapon equip
        if (Input.GetKeyDown(KeyCode.W))
        {
            player.EquipWeapon(6);
        }
    }

    void TestCombat()
    {
        List<CardTest> testCards = new List<CardTest> {
            new CardTest("Clover", 4),
            new CardTest("Spade", 10),
            new CardTest("Heart", 5),
            new CardTest("Diamond", 6)
        };

        Debug.Log($"[CombatTester] Starting combat with {testCards.Count} cards");
        combatManager.StartCombat(testCards);
    }
}