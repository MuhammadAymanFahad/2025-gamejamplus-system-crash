using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MonsterUISlot : MonoBehaviour
{
    [Header("UI References - Leave Empty, Auto-Assigned")]
    public TextMeshProUGUI monsterInfoText;
    public Button attackButton;
    public Image monsterIcon;

    public CombatCard Monster { get; private set; }
    private Action<CombatCard> onAttackCallback;

    void Awake()
    {
        // ✅ Auto-find by name (MOST RELIABLE)
        if (monsterInfoText == null)
        {
            Transform infoObj = transform.Find("MonsterInfoText");
            if (infoObj != null)
            {
                monsterInfoText = infoObj.GetComponent<TextMeshProUGUI>();
            }
        }

        if (attackButton == null)
        {
            Transform buttonObj = transform.Find("AttackButton");
            if (buttonObj != null)
            {
                attackButton = buttonObj.GetComponent<Button>();
            }
        }

        // Debug verification
        if (monsterInfoText == null)
        {
            Debug.LogError($"[{gameObject.name}] MonsterInfoText not found! Check GameObject name is 'MonsterInfo'");
        }

        if (attackButton == null)
        {
            Debug.LogError($"[{gameObject.name}] AttackButton not found! Check GameObject name is 'AttackButton'");
        }
    }

    public void Initialize(CombatCard monster, Action<CombatCard> attackCallback)
    {
        Monster = monster;
        onAttackCallback = attackCallback;

        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners(); // Clear previous
            attackButton.onClick.AddListener(OnAttackClicked);
        }

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (monsterInfoText == null) return;

        string monsterType = Monster.suit == "Clover" ? "♣" : "♠";
        string bossLabel = Monster.isBoss ? " [BOSS]" : "";

        monsterInfoText.text = $"HP: {Monster.currentHP}/{Monster.grade}";

        if (Monster.isBoss)
        {
            monsterInfoText.color = Color.red;
        }
        else if (Monster.currentHP < Monster.grade * 0.5f)
        {
            monsterInfoText.color = Color.yellow;
        }
        else
        {
            monsterInfoText.color = Color.white;
        }
    }

    void OnAttackClicked()
    {
        onAttackCallback?.Invoke(Monster);
    }

    public void SetAttackButtonInteractable(bool interactable)
    {
        if (attackButton != null)
        {
            attackButton.interactable = interactable;
        }
    }
}