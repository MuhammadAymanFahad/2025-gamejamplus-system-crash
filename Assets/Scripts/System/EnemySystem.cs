using System.Collections;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGameAction>(EnemyTurnPerform);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGameAction>();
    }
    private IEnumerator EnemyTurnPerform(EnemyTurnGameAction enemyTurnGameAction)
    {
        Debug.Log("Enemy Turn");
        yield return new WaitForSeconds(2f);
        Debug.Log("End Enemy Turn");
    }
}
