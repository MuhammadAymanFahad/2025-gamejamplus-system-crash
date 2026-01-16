using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References (assign in Inspector or leave null to auto-find)")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private FloorManager floorManager;

    [Header("Startup")]
    [SerializeField] private int startFloorIndex = 0;
    [SerializeField] private bool autoBuildAndSetup = true;

    private void Start()
    {
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManager>();
        }
    }
}