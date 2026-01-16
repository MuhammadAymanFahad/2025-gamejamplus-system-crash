using UnityEngine;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;

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
        if ( floorManager == null )
        {
            floorManager = Object.FindFirstObjectByType<FloorManager>();
            if ( floorManager == null)
            {
                Debug.LogError("FloorManager not found in the scene. Make sure FloorManager GameObject Exists");
                return;
            }

            if(playerInventory == null )
            {
                playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
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
            if ( roomCardIndex.state != CardRevealState.Hidden)
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
    }
}