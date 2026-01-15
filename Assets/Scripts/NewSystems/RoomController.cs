using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class RoomController : MonoBehaviour
{
    public int roomIndex = 0;
    public Button[] cardButtons;

    private FloorManager floorManager;
    private RoomState roomState;

    private void Start()
    {
        floorManager = FindObjectOfType<FloorManager>();
        if (floorManager == null)
        {
            Debug.LogError("FloorManager not found in the scene.");
            return;
        }
        
        if (floorManager.rooms != null && floorManager.rooms.Length > roomIndex)
        {
            roomState = floorManager.rooms[roomIndex];
        } 
        else {
            Debug.LogError($"RoomState for index {roomIndex} not found. Make sure FloorManager.SetupFloor was called.");
            return;
        }

        if (cardButtons == null || cardButtons.Length == 0)
        {
            var btns = GetComponentsInChildren<Button>();
            cardButtons = new Button[btns.Length];
            for (int i = 0; i < btns.Length; i++)
            {
                cardButtons[i] = btns[i];
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

    public void OnCardClicked(int cardIndex)
    {
        if (roomState == null)
        {
            return;
        }

        if(cardIndex < 0 || cardIndex >= roomState.cards.Count)
        {
            Debug.LogWarning("Card index out of range or card missing.");
            return;
        }

        var roomCardIndex = roomState.cards[cardIndex];
        if (roomCardIndex.state != CardRevealState.Hidden)
        {
            Debug.Log("Card already revealed/resolved");
            return;
        }

        roomCardIndex.state = CardRevealState.Revealed;
        Debug.Log($"Revealed card in room {roomIndex} slot {cardIndex}: type={roomCardIndex.cardData.type}, grade={roomCardIndex.cardData.grade}");

        switch (roomCardIndex.cardData.type)
        {
            case CardType.Heart:
                PlayerInventory.Instance.AddPotion(roomCardIndex.cardData);
                roomCardIndex.state = CardRevealState.Resolved;
                break;

            case CardType.Diamond:
                // For now: auto-pickup. Later show UI confirm pick/abandon.
                PlayerInventory.Instance.PickupWeapon(roomCardIndex.cardData);
                roomCardIndex.state = CardRevealState.Resolved;
                break;

            case CardType.Joker:
                // implement relic application elsewhere; for now just resolve
                Debug.Log("Joker revealed - apply relic later");
                roomCardIndex.state = CardRevealState.Resolved;
                break;

            case CardType.Spade:
                // Enemy: start combat and wait for callback
                StartCombatForCard(cardIndex, roomCardIndex.cardData);
                return; // don't call UpdateCompletion until combat callback


            case CardType.Clover:
                // Enemy: start combat and wait for callback
                StartCombatForCard(cardIndex, roomCardIndex.cardData);
                return; // don't call UpdateCompletion until combat callback
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
        
    }

    void UpdateCardVisual(int cardIndex)
    {
        // Implement visual update logic here
    }

    void OnRoomCompleted()
    {
        Debug.Log($"Room {roomIndex} completed!");
    }
}
