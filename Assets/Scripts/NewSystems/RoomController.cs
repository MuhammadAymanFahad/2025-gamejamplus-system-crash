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
    }


}
