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
    public GameObject roomCardSlotPrefab; // Generic card slot prefab

    [Header("Action Buttons")]
    public Button fleeButton;

    // Runtime data
    private List<RoomCardUISlot> roomCardSlots = new List<RoomCardUISlot>();
    private List<CombatCard> currentRoomCards = new List<CombatCard>();
    private List<RoomCardData> originalRoomCardData = new List<RoomCardData>(); // For sprites

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
        fleeButton.onClick.AddListener(OnFleeClicked);

        // Subscribe to events
        CombatEvents.OnCombatEnd += OnCombatEnd;
    }

    void OnDestroy()
    {
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

    /// <summary>
    /// Show ALL room cards (monsters, potions, weapons)
    /// </summary>
    public void ShowCombat(List<CombatCard> roomCards, List<RoomCardData> originalData = null)
    {
        Debug.Log($"[CombatUI] ===== ShowCombat CALLED ===== with {roomCards.Count} cards");

        gameObject.SetActive(true);
        currentRoomCards = roomCards;
        originalRoomCardData = originalData ?? new List<RoomCardData>();

        // Clear old card slots
        ClearCardSlots();

        // Create UI slot for EACH card
        Debug.Log($"[CombatUI] Creating {roomCards.Count} card slots...");
        for (int i = 0; i < roomCards.Count; i++)
        {
            CombatCard card = roomCards[i];
            RoomCardData cardData = (originalData != null && i < originalData.Count) ? originalData[i] : null;

            Debug.Log($"[CombatUI] Creating slot {i}: {card.suit} {card.grade} (Monster: {card.isMonster})");
            CreateCardSlot(card, cardData);
        }

        Debug.Log($"[CombatUI] Total card slots created: {roomCardSlots.Count}");

        UpdatePlayerStats();
        UpdateTurnIndicator();

        Debug.Log("[CombatUI] ===== ShowCombat COMPLETE =====");
    }

    void CreateCardSlot(CombatCard card, RoomCardData cardData = null)
    {
        Debug.Log($"[CombatUI] CreateCardSlot START - Prefab null? {roomCardSlotPrefab == null}, Container null? {roomCardsContainer == null}");

        if (roomCardSlotPrefab == null)
        {
            Debug.LogError("[CombatUI] roomCardSlotPrefab is NULL!");
            return;
        }

        if (roomCardsContainer == null)
        {
            Debug.LogError("[CombatUI] roomCardsContainer is NULL!");
            return;
        }

        Debug.Log("[CombatUI] Instantiating card slot prefab...");
        GameObject slotObj = Instantiate(roomCardSlotPrefab, roomCardsContainer);
        Debug.Log($"[CombatUI] Slot instantiated: {slotObj.name}");

        RoomCardUISlot slot = slotObj.GetComponent<RoomCardUISlot>();
        if (slot == null)
        {
            Debug.LogError("[CombatUI] RoomCardUISlot component not found!");
            return;
        }

        Debug.Log("[CombatUI] Initializing slot...");
        if (cardData != null)
        {
            // Initialize with sprite data
            slot.Initialize(card, cardData, OnCardClicked);
        }
        else
        {
            // Initialize without sprite
            slot.Initialize(card, OnCardClicked);
        }

        roomCardSlots.Add(slot);
        Debug.Log($"[CombatUI] Slot added. Total slots: {roomCardSlots.Count}");
    }

    void ClearCardSlots()
    {
        foreach (var slot in roomCardSlots)
        {
            Destroy(slot.gameObject);
        }
        roomCardSlots.Clear();
    }

    /// <summary>
    /// Handle card click based on card type
    /// </summary>
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
            // Attack monster
            Debug.Log($"[CombatUI] Attacking monster grade {card.grade}");
            combatManager.PlayerAttack(card);
            UpdateCardSlots();
        }
        else if (card.suit == "Heart")
        {
            // Use potion
            Debug.Log($"[CombatUI] Using potion grade {card.grade}");
            combatManager.PlayerHeal(card.grade);
            RemoveUsedCard(card);
        }
        else if (card.suit == "Diamond")
        {
            // Equip weapon
            Debug.Log($"[CombatUI] Equipping weapon grade {card.grade}");
            player.EquipWeapon(card.grade);
            RemoveUsedCard(card);
        }
    }

    void RemoveUsedCard(CombatCard card)
    {
        // Find and remove slot
        RoomCardUISlot slotToRemove = roomCardSlots.Find(s => s.GetCard() == card);
        if (slotToRemove != null)
        {
            roomCardSlots.Remove(slotToRemove);
            Destroy(slotToRemove.gameObject);
            Debug.Log($"[CombatUI] Removed card from UI: {card.suit} {card.grade}");
        }

        // Remove from current cards list
        currentRoomCards.Remove(card);
    }

    void UpdateCardSlots()
    {
        foreach (var slot in roomCardSlots)
        {
            slot.UpdateHP();
        }
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

    void UpdateActionButtons()
    {
        bool isPlayerTurn = turnManager.IsPlayerTurn();
        fleeButton.interactable = isPlayerTurn && player.CanFlee;

        //// Update card button interactability
        //foreach (var slot in roomCardSlots)
        //{
        //    // Enable/disable based on turn
        //    slot.cardButton.interactable = isPlayerTurn;
        //}
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