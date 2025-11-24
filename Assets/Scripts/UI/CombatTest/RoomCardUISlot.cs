using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// Card UI slot - displays card sprite with type label and HP (for monsters)
/// Click directly on card to interact
/// </summary>
public class RoomCardUISlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Essential UI")]
    [Tooltip("Main card image - displays sprite from RoomCardData")]
    public Image cardImage;

    [Tooltip("Type label above/on card (MONSTER/HEAL/WEAPON)")]
    public TextMeshProUGUI typeText;

    [Tooltip("HP display - only shown for monsters")]
    public TextMeshProUGUI hpText;

    [Header("Optional Visual Feedback")]
    [Tooltip("Shadow components for glow effect on hover")]
    public Shadow[] glowShadows;

    [Header("Glow Colors")]
    public Color monsterGlow = new Color(1f, 0.3f, 0.3f, 0.8f); // Red
    public Color potionGlow = new Color(0.3f, 1f, 0.3f, 0.8f);  // Green
    public Color weaponGlow = new Color(0.3f, 0.7f, 1f, 0.8f);  // Blue

    private CombatCard card;
    private RoomCardData cardData;
    private Action<CombatCard> onCardClicked;
    private bool isInteractable = true;

    /// <summary>
    /// Initialize without sprite data
    /// </summary>
    public void Initialize(CombatCard combatCard, Action<CombatCard> clickCallback)
    {
        card = combatCard;
        onCardClicked = clickCallback;
        UpdateDisplay();
    }

    /// <summary>
    /// Initialize with sprite data from RoomCardData
    /// </summary>
    public void Initialize(CombatCard combatCard, RoomCardData roomCardData, Action<CombatCard> clickCallback)
    {
        card = combatCard;
        cardData = roomCardData;
        onCardClicked = clickCallback;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (card == null)
        {
            Debug.LogError("[RoomCardUI] Card is null!");
            return;
        }

        // ===== CARD IMAGE (Main Visual) =====
        if (cardImage != null)
        {
            if (cardData != null && cardData.cardSprite != null)
            {
                // Use sprite from ScriptableObject
                cardImage.sprite = cardData.cardSprite;
                cardImage.color = Color.white;
            }
            else
            {
                // Fallback: solid color if no sprite
                cardImage.sprite = null;
                cardImage.color = GetFallbackColor();
            }
        }
        else
        {
            Debug.LogError("[RoomCardUI] cardImage is null! Assign in Inspector.");
        }

        // ===== TYPE TEXT (Label) =====
        if (typeText != null)
        {
            if (card.isMonster)
            {
                typeText.text = "MONSTER";
                typeText.color = Color.red;
            }
            else if (card.suit == "Heart")
            {
                typeText.text = "HEAL";
                typeText.color = Color.green;
            }
            else if (card.suit == "Diamond")
            {
                typeText.text = "WEAPON";
                typeText.color = Color.cyan;
            }
            else
            {
                typeText.text = "CARD";
                typeText.color = Color.white;
            }
        }

        // ===== HP TEXT (Only for Monsters) =====
        if (hpText != null)
        {
            if (card.isMonster)
            {
                hpText.gameObject.SetActive(true);
                hpText.text = $"HP: {card.currentHP}";
                hpText.color = Color.white;
            }
            else
            {
                // Hide HP for non-monsters
                hpText.gameObject.SetActive(false);
            }
        }

        // ===== GLOW EFFECT SETUP =====
        if (glowShadows != null && glowShadows.Length > 0)
        {
            Color glowColor = GetGlowColor();
            foreach (var shadow in glowShadows)
            {
                if (shadow != null)
                {
                    shadow.effectColor = glowColor;
                    shadow.enabled = false; // Disabled by default
                }
            }
        }
    }

    /// <summary>
    /// Update HP text when monster takes damage
    /// </summary>
    public void UpdateHP()
    {
        if (card != null && card.isMonster && hpText != null && hpText.gameObject.activeSelf)
        {
            hpText.text = $"HP: {card.currentHP}";

            // Optional: Change color based on HP
            if (card.currentHP <= 0)
            {
                hpText.color = Color.gray;
            }
            else if (card.currentHP <= card.grade * 0.3f)
            {
                hpText.color = Color.red;
            }
            else if (card.currentHP <= card.grade * 0.6f)
            {
                hpText.color = Color.yellow;
            }
            else
            {
                hpText.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Enable/disable card interaction
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        // Optional: Visual feedback when not interactable
        if (cardImage != null)
        {
            cardImage.color = interactable ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }
    }

    public CombatCard GetCard() => card;

    // ===== CLICK HANDLER =====
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable)
        {
            Debug.Log("[RoomCardUI] Card not interactable!");
            return;
        }

        Debug.Log($"[RoomCardUI] Card clicked: {card.suit} {card.grade}");
        onCardClicked?.Invoke(card);
    }

    // ===== HOVER ENTER - Show Glow =====
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;

        // Enable all shadow components for glow effect
        if (glowShadows != null)
        {
            foreach (var shadow in glowShadows)
            {
                if (shadow != null)
                    shadow.enabled = true;
            }
        }

        // Optional: Scale up slightly for feedback
        transform.localScale = Vector3.one * 1.05f;
    }

    // ===== HOVER EXIT - Hide Glow =====
    public void OnPointerExit(PointerEventData eventData)
    {
        // Disable all shadow components
        if (glowShadows != null)
        {
            foreach (var shadow in glowShadows)
            {
                if (shadow != null)
                    shadow.enabled = false;
            }
        }

        // Reset scale
        transform.localScale = Vector3.one;
    }

    // ===== HELPER METHODS =====

    /// <summary>
    /// Fallback color when no sprite is assigned
    /// </summary>
    Color GetFallbackColor()
    {
        if (card.isMonster) return new Color(0.8f, 0.2f, 0.2f); // Red
        if (card.suit == "Heart") return new Color(0.2f, 0.8f, 0.2f); // Green
        if (card.suit == "Diamond") return new Color(0.2f, 0.5f, 0.8f); // Blue
        return Color.gray;
    }

    /// <summary>
    /// Get glow color based on card type
    /// </summary>
    Color GetGlowColor()
    {
        if (card.isMonster) return monsterGlow;
        if (card.suit == "Heart") return potionGlow;
        if (card.suit == "Diamond") return weaponGlow;
        return Color.white;
    }
}