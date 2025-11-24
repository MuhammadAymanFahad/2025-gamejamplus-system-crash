using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI turnIndicatorText;

    [Header("Room Cards Panel")]
    public Transform roomCardsContainer; // Container for ALL 3 cards
    public GameObject roomCardSlotPrefab;

    [Header("Action Buttons")]
    public Button fleeButton;

    // Runtime data
    private List<RoomCardUISlot> roomCardSlots = new List<RoomCardUISlot>();
    private List<CombatCard> currentRoomCards = new List<CombatCard>();

    private CombatManager combatManager;
    private PlayerManager player;
    private TurnManager turnManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        combatManager = CombatManager.Instance;
        player = PlayerManager.Instance;
        turnManager = combatManager.GetComponent<TurnManager>();

        // Setup button listeners
        fleeButton.onClick.AddListener(OnFleeClicked);

        // Subscribe to events
        CombatEvents.OnCombatEnd += OnCombatEnd;
        CombatEvents.OnMonsterKilled += OnMonsterKilled; // <-- ADDED
        CombatEvents.OnBossKilled += OnBossKilled;       // <-- ADDED (optional)
    }

    void OnDestroy()
    {
        CombatEvents.OnCombatEnd -= OnCombatEnd;
        CombatEvents.OnMonsterKilled -= OnMonsterKilled; // <-- ADDED
        CombatEvents.OnBossKilled -= OnBossKilled;       // <-- ADDED
    }

    void Update()
    {
        if (combatManager.IsInCombat())
        {
            UpdatePlayerStats();
            UpdateTurnIndicator();
            //UpdateActionButtons();
        }
    }

    public void ShowCombat(List<CombatCard> roomCards)
    {
        Debug.Log($"[CombatUI] ===== ShowCombat CALLED ===== with {roomCards.Count} cards");

        gameObject.SetActive(true);
        currentRoomCards = roomCards;

        ClearCardSlots();

        foreach (CombatCard card in roomCards)
        {
            CreateCardSlot(card);
        }

        UpdatePlayerStats();
        UpdateTurnIndicator();

        Debug.Log("[CombatUI] ===== ShowCombat COMPLETE =====");
    }

    void CreateCardSlot(CombatCard card)
    {
        if (roomCardSlotPrefab == null || roomCardsContainer == null)
        {
            Debug.LogError("[CombatUI] Prefab or container NULL!");
            return;
        }

        GameObject slotObj = Instantiate(roomCardSlotPrefab, roomCardsContainer);
        RoomCardUISlot slot = slotObj.GetComponent<RoomCardUISlot>();
        if (slot == null)
        {
            Debug.LogError("[CombatUI] RoomCardUISlot component not found!");
            Destroy(slotObj);
            return;
        }

        slot.Initialize(card, OnCardClicked);
        roomCardSlots.Add(slot);
    }

    void ClearCardSlots()
    {
        foreach (var slot in roomCardSlots) Destroy(slot.gameObject);
        roomCardSlots.Clear();
    }

    void OnCardClicked(CombatCard card)
    {
        if (!turnManager.IsPlayerTurn())
        {
            Debug.Log("[CombatUI] Not player's turn!");
            return;
        }

        Debug.Log($"[CombatUI] Card clicked: {card.suit} {card.grade}");

        if (card.isMonster)
        {
            combatManager.PlayerAttack(card);
            UpdateCardSlots();
        }
        else if (card.suit == "Heart")
        {
            combatManager.PlayerHeal(card.grade);
            RemoveUsedCard(card);
        }
        else if (card.suit == "Diamond")
        {
            player.EquipWeapon(card.grade);
            RemoveUsedCard(card);
        }
    }

    // Robust Remove: try reference match first, fallback to logical match (suit+grade + hp <= 0)
    void RemoveUsedCard(CombatCard card)
    {
        RoomCardUISlot slotToRemove = null;

        if (card != null)
            slotToRemove = roomCardSlots.Find(s => s.GetCard() == card);

        if (slotToRemove == null && card != null)
        {
            // Fallback: match by suit+grade and low HP (useful for monsters)
            slotToRemove = roomCardSlots.Find(s =>
            {
                var c = s.GetCard();
                if (c == null) return false;
                // If reference didn't match, use identity + hp state
                if (c.suit == card.suit && c.grade == card.grade)
                {
                    // if monster, prefer hp <= 0; for non-monsters we can remove anyway
                    if (c.isMonster) return c.currentHP <= 0;
                    return true;
                }
                return false;
            });
        }

        if (slotToRemove != null)
        {
            roomCardSlots.Remove(slotToRemove);
            Destroy(slotToRemove.gameObject);
            Debug.Log($"[CombatUI] Removed card from UI: {card.suit} {card.grade}");
        }
        else
        {
            Debug.LogWarning($"[CombatUI] Tried to remove card but slot not found: {card?.suit} {card?.grade}");
        }

        // Remove from current cards list as well (safe-remove)
        if (card != null)
        {
            currentRoomCards.RemoveAll(c => c == card || (c.suit == card.suit && c.grade == card.grade && c.currentHP <= 0));
        }
    }

    void UpdateCardSlots()
    {
        foreach (var slot in roomCardSlots) slot.UpdateHP();
    }

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

    // Event handler: monster died
    void OnMonsterKilled(CombatCard monster)
    {
        Debug.Log($"[CombatUI] OnMonsterKilled received for {monster.suit} {monster.grade}");
        RemoveUsedCard(monster);
        UpdateCardSlots();
    }

    // Event handler: boss died (optional behaviour - remove boss and optionally clear floor)
    void OnBossKilled(CombatCard boss)
    {
        Debug.Log($"[CombatUI] OnBossKilled received for {boss.suit} {boss.grade}");
        RemoveUsedCard(boss);
    }

    void OnFleeClicked()
    {
        if (!turnManager.IsPlayerTurn()) return;

        player.Flee();
        HideUI();
    }

    void OnCombatEnd()
    {
        HideUI();
    }

    void HideUI()
    {
        gameObject.SetActive(false);
    }
}