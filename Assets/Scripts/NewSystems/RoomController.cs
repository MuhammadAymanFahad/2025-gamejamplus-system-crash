using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class RoomController : MonoBehaviour
{
    [Header("Room Setup")]
    public int roomIndex = 0;
    public Button[] cardButtons;

    [Header("Dependencies (assign in Inspector or leave null to auto find)")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private FloorManager floorManager;

    private RoomState roomState;

    private void Start()
    {
        if (floorManager == null)
        {
            // Use the newest API if available, else fallback to older FindObjectOfType
            floorManager = UnityEngine.Object.FindFirstObjectByType<FloorManager>();
            if (floorManager == null)
            {
                Debug.LogError("FloorManager not found in the scene. Make sure FloorManager GameObject Exists");
                return;
            }

            if (playerInventory == null)
            {
                playerInventory = UnityEngine.Object.FindFirstObjectByType<PlayerInventory>();
            }

            if (cardButtons == null || cardButtons.Length == 0)
            {
                var buttons = GetComponentsInChildren<Button>();
                cardButtons = new Button[buttons.Length];
                for (int i = 0; i < buttons.Length; i++)
                {
                    cardButtons[i] = buttons[i];
                }
            }

            for (int i = 0; i < cardButtons.Length; i++)
            {
                int index = i;
                cardButtons[i].onClick.RemoveAllListeners();
                cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
                UpdateCardVisual(index);
            }
        }
    }

    public void OnCardClicked(int cardIndex)
    {
        if (roomState == null)
        {
            return;
        }

        if (cardIndex < 0 || cardIndex >= roomState.cards.Count)
        {
            Debug.LogWarning("Card Indexout of range or card missing");
            return;
        }

        var roomCardIndex = roomState.cards[cardIndex];
        if (roomCardIndex.state != CardRevealState.Hidden)
        {
            Debug.Log("Card already revealed or resolved");
            return;
        }

        roomCardIndex.state = CardRevealState.Revealed;
        Debug.Log($"Revealed card room {roomIndex} slot {cardIndex}: type={roomCardIndex.cardData.type}, grade= {roomCardIndex.cardData.grade}");

        switch (roomCardIndex.cardData.type)
        {
            case CardType.Heart:
                playerInventory?.AddPotion(roomCardIndex.cardData);
                roomCardIndex.state = CardRevealState.Resolved;
                break;

            case CardType.Diamond:
                playerInventory?.PickupWeapon(roomCardIndex.cardData);
                roomCardIndex.state = CardRevealState.Resolved;
                break;

            case CardType.Spade:
                StartCombatForCard(cardIndex, roomCardIndex.cardData);
                return;

            case CardType.Clover:
                StartCombatForCard(cardIndex, roomCardIndex.cardData);
                return;
        }

        UpdateCardVisual(cardIndex);
        roomState.UpdateCompletion();

        if (roomState.isCompleted)
        {
            OnRoomCompleted();
        }
    }

    void StartCombatForCard(int cardIndex, CardSO enemyCard)
    {
        // Find a CombatBridge instance (no static Instance required)
        var combatBridgeInstance = UnityEngine.Object.FindFirstObjectByType<CombatBridge>();

        if (combatBridgeInstance == null)
        {
            Debug.Log("CombatBridge not found: simulating instant victory (dev mode).");
            playerInventory?.AddWeaponFromEnemy(enemyCard.grade);
            roomState.cards[cardIndex].state = CardRevealState.Resolved;
            UpdateCardVisual(cardIndex);
            roomState.UpdateCompletion();

            if (roomState.isCompleted)
            {
                OnRoomCompleted();
            }

            return;
        }

        combatBridgeInstance.StartCombat(enemyCard, (result) =>
        {
            if (result == null) return;

            if (result.outcome == CombatOutcome.Victory)
            {
                playerInventory?.AddWeaponFromEnemy(result.monsterGrade);
                roomState.cards[cardIndex].state = CardRevealState.Resolved;
            }
            else if (result.outcome == CombatOutcome.Fled && result.fledSuccess)
            {
                Debug.Log("Player fled successfully.");
            }
            else if (result.outcome == CombatOutcome.Defeat)
            {
                Debug.Log("Player defeated - implement death flow.");
            }

            UpdateCardVisual(cardIndex);
            roomState.UpdateCompletion();
            if (roomState.isCompleted) OnRoomCompleted();
        });
    }

    void UpdateCardVisual(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= cardButtons.Length)
        {
            return;
        }

        var button = cardButtons[cardIndex];
        var text = button.GetComponentInChildren<UnityEngine.UI.Text>();

        if (text == null)
        {
            return;
        }

        var roomCardIndex = (cardIndex < roomState.cards.Count) ? roomState.cards[cardIndex] : null;
        if (roomCardIndex == null)
        {
            text.text = "N/A";
        }
        else
        {
            if (roomCardIndex.state == CardRevealState.Hidden)
            {
                text.text = "Face Down";
            }
            else if (roomCardIndex.state == CardRevealState.Revealed)
            {
                text.text = $"{roomCardIndex.cardData.type} {roomCardIndex.cardData.grade}";
            }
            else
            {
                text.text = $"Resolved";
            }
        }
    }

    void OnRoomCompleted()
    {
        Debug.Log($"Room {roomIndex} completed!");
    }
}