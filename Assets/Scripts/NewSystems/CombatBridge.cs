using UnityEngine;
using System;

public enum CombatOutcome { Victory, Fled, Defeat }

public class CombatResult
{
    public CombatOutcome outcome;
    public int monsterGrade;
    public bool fledSuccess;
}

public class CombatBridge : MonoBehaviour
{
    // Optional singleton — not required by RoomController after perbaikan,
    // tetapi handy if bagian lain memakai CombatBridge.Instance
    public static CombatBridge Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // if you don't want multiple instances, destroy duplicates
            Destroy(gameObject);
        }
    }

    // Minimal StartCombat signature expected by RoomController:
    // enemyCard: CardSO referencing the enemy encountered
    // onComplete: callback invoked with CombatResult when combat finishes
    public void StartCombat(CardSO enemyCard, Action<CombatResult> onComplete)
    {
        // DEVELOPMENT placeholder: immediately return Victory
        Debug.Log($"[CombatBridge] Simulating combat vs grade {enemyCard.grade}");

        var result = new CombatResult
        {
            outcome = CombatOutcome.Victory,
            monsterGrade = enemyCard.grade,
            fledSuccess = false
        };

        // invoke callback on main thread immediately
        onComplete?.Invoke(result);

        // Real implementation outline:
        // 1) Store the callback in a field.
        // 2) Load the combat scene additively: SceneManager.LoadScene("CombatScene", LoadSceneMode.Additive)
        // 3) Pass enemy data to CombatManager.
        // 4) When combat ends, CombatManager calls a method on CombatBridge to finish (invoke callback and unload scene).
    }

    // Optional helper to be called by real CombatManager when combat finishes:
    public void CompleteCombat(CombatResult result, Action<CombatResult> onComplete = null)
    {
        onComplete?.Invoke(result);
    }
}