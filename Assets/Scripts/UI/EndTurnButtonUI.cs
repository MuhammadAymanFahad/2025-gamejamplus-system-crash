using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{
    public void OnClick()
    {
        EnemyTurnGameAction enemyTurnGA = new();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }
}
