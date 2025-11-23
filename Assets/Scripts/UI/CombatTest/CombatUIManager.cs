using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // or using UnityEngine.UI for Text

public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI turnIndicatorText;

    [Header("Monsters Panel")]
    public Transform monstersContainer;
    public GameObject monsterSlotPrefab;

    [Header("Action Buttons")]
    public Button usePotionButton;
    public Button discardWeaponButton;
    public Button fleeButton;

    // Runtime data
    private List<MonsterUISlot> monsterSlots = new List<MonsterUISlot>();
    private CombatManager combatManager;
    private PlayerManager player;
    private TurnManager turnManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        combatManager = CombatManager.Instance;
        player = PlayerManager.Instance;
        turnManager = combatManager.GetComponent<TurnManager>();

        // Setup button listeners
        usePotionButton.onClick.AddListener(OnUsePotionClicked);
        //discardWeaponButton.onClick.AddListener(OnDiscardWeaponClicked);
        fleeButton.onClick.AddListener(OnFleeClicked);

        // Subscribe to events
        CombatEvents.OnMonsterKilled += OnMonsterKilled;
        CombatEvents.OnCombatEnd += OnCombatEnd;

        // Hide UI initially
        HideUI();
    }

    void OnDestroy()
    {
        CombatEvents.OnMonsterKilled -= OnMonsterKilled;
        CombatEvents.OnCombatEnd -= OnCombatEnd;
    }

    void Update()
    {
        if (combatManager.IsInCombat())
        {
            UpdatePlayerStats();
            UpdateTurnIndicator();
            UpdateActionButtons();
        }
    }

    // ✅ Called by CombatManager when combat starts
    public void ShowCombat(List<CombatCard> monsters)
    {
        gameObject.SetActive(true);

        // Clear old monster slots
        ClearMonsterSlots();

        // Create UI slots for each monster
        foreach (CombatCard monster in monsters)
        {
            CreateMonsterSlot(monster);
        }

        UpdatePlayerStats();
        UpdateTurnIndicator();
    }

    void CreateMonsterSlot(CombatCard monster)
    {
        GameObject slotObj = Instantiate(monsterSlotPrefab, monstersContainer);
        MonsterUISlot slot = slotObj.GetComponent<MonsterUISlot>();
        slot.Initialize(monster, OnMonsterAttackClicked);
        monsterSlots.Add(slot);
    }

    void ClearMonsterSlots()
    {
        foreach (var slot in monsterSlots)
        {
            Destroy(slot.gameObject);
        }
        monsterSlots.Clear();
    }

    // ✅ Update player stats display
    void UpdatePlayerStats()
    {
        float hpPercent = (float)player.CurrentHP / player.MaxHP;
        Color hpColor = Color.white;

        if (hpPercent < 0.3f) hpColor = Color.red;
        else if (hpPercent < 0.6f) hpColor = Color.yellow;
        else hpColor = Color.green;

        hpText.text = $"HP: {player.CurrentHP}/{player.MaxHP}";
        hpText.color = hpColor;

        if (player.WeaponGrade > 0)
        {
            var stats = player.GetComponent<PlayerStats>();
            weaponText.text = $"Weapon: Grade {player.WeaponGrade}";
            weaponText.color = Color.cyan;
        }
        else
        {
            weaponText.text = "Barehanded";
            weaponText.color = Color.gray;
        }
    }

    void UpdateTurnIndicator()
    {
        if (!turnManager.IsPlayerTurn())
        {
            turnIndicatorText.text = "MONSTER TURN...";
            turnIndicatorText.color = Color.red;
        }
        else
        {
            turnIndicatorText.text = "YOUR TURN";
            turnIndicatorText.color = Color.green;
        }
    }

    void UpdateActionButtons()
    {
        bool isPlayerTurn = turnManager.IsPlayerTurn();

        // Enable buttons only during player turn
        usePotionButton.interactable = isPlayerTurn;
        //discardWeaponButton.interactable = isPlayerTurn && player.WeaponGrade > 0;
        fleeButton.interactable = isPlayerTurn && player.CanFlee;

        // Update monster attack buttons
        foreach (var slot in monsterSlots)
        {
            slot.SetAttackButtonInteractable(isPlayerTurn);
        }
    }

    // ✅ Button callbacks
    void OnMonsterAttackClicked(CombatCard monster)
    {
        if (!turnManager.IsPlayerTurn()) return;

        Debug.Log($"Attacking monster grade {monster.grade}");
        combatManager.PlayerAttack(monster);

        // Update monster UI after attack
        UpdateMonsterSlots();
    }

    void OnUsePotionClicked()
    {
        if (!turnManager.IsPlayerTurn()) return;

        // TODO: Show potion selection UI
        // For now, use fixed value
        combatManager.PlayerHeal(5);
    }

    void OnDiscardWeaponClicked()
    {
        if (!turnManager.IsPlayerTurn()) return;

        player.DiscardWeapon();
        UpdatePlayerStats();
    }

    void OnFleeClicked()
    {
        if (!turnManager.IsPlayerTurn()) return;

        player.Flee();
        HideUI();
        // TODO: Trigger return to floor scene
    }

    // ✅ Event handlers
    void OnMonsterKilled(CombatCard monster)
    {
        UpdateMonsterSlots();
    }

    void OnCombatEnd()
    {
        HideUI();
    }

    void UpdateMonsterSlots()
    {
        var activeMonsters = turnManager.GetActiveMonsters();

        // Remove dead monsters from UI
        for (int i = monsterSlots.Count - 1; i >= 0; i--)
        {
            if (!activeMonsters.Contains(monsterSlots[i].Monster))
            {
                Destroy(monsterSlots[i].gameObject);
                monsterSlots.RemoveAt(i);
            }
            else
            {
                monsterSlots[i].UpdateDisplay();
            }
        }
    }

    void HideUI()
    {
        gameObject.SetActive(false);
    }


}