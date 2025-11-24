using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// Ultra-clean card UI - click directly on card image
/// Shows sprite + HP (monsters only)
/// Optional glow on hover for visual feedback
/// </summary>
public class RoomCardUISlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Essential UI")]
    public Image cardImage; // Main card visual (MUST HAVE)
    public TextMeshProUGUI hpText; // Only shown for monsters

    [Header("Optional Visual Feedback")]
    [Tooltip("Optional: Shadow components on CardImage for glow effect")]
    public Shadow[] glowShadows; // Multiple shadows for glow effect

    [Header("Glow Colors")]
    public Color monsterGlow = new Color(1f, 0.3f, 0.3f, 0.5f); // Red
    public Color potionGlow = new Color(0.3f, 1f, 0.3f, 0.5f);  // Green
    public Color weaponGlow = new Color(0.3f, 0.7f, 1f, 0.5f);  // Blue

    private CombatCard card;
    private RoomCardData cardData;
    private Action<CombatCard> onCardClicked;
    private bool isInteractable = true;

    public void Initialize(CombatCard combatCard, Action<CombatCard> clickCallback)
    {
        card = combatCard;
        onCardClicked = clickCallback;
        UpdateDisplay();
    }

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

        // ✅ Set card sprite (main visual)
        if (cardImage != null)
        {
            if (cardData != null && cardData.cardSprite != null)
            {
                // Use sprite from artist
                cardImage.sprite = cardData.cardSprite;
                cardImage.color = Color.white;
            }
        }
        else
        {
            Debug.LogError("[RoomCardUI] cardImage is null! Assign in Inspector.");
        }

        // ✅ HP text - ONLY for monsters
        if (hpText != null)
        {
            if (card.isMonster)
            {
                hpText.gameObject.SetActive(true);
                hpText.text = $"HP : {card.currentHP}";
                hpText.color = monsterGlow; 
            }
            else
            {
                hpText.gameObject.SetActive(false);
            }
        }

        // ✅ Setup glow shadows (if exists)
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

    // ✅ Click handler - No Button needed!
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable) return;

        Debug.Log($"[RoomCardUI] Card clicked: {card.suit} {card.grade}");
        onCardClicked?.Invoke(card);
    }

    // ✅ Hover enter - Show glow
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

        // Optional: Scale up slightly
        transform.localScale = Vector3.one * 1.05f;
    }

    // ✅ Hover exit - Hide glow
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

    public void UpdateHP()
    {
        if (card.isMonster && hpText != null && hpText.gameObject.activeSelf)
        {
            hpText.text = $"HP: {card.currentHP}";
        }
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;

        // Optional: Darken when not interactable
        if (cardImage != null)
        {
            cardImage.color = interactable ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }
    }

    public CombatCard GetCard() => card;

    // Fallback colors when no sprite
    Color GetFallbackColor()
    {
        if (card.isMonster) return new Color(0.8f, 0.2f, 0.2f); // Red
        if (card.suit == "Heart") return new Color(0.2f, 0.8f, 0.2f); // Green
        if (card.suit == "Diamond") return new Color(0.2f, 0.5f, 0.8f); // Blue
        return Color.gray;
    }

    Color GetGlowColor()
    {
        if (card.isMonster) return monsterGlow;
        if (card.suit == "Heart") return potionGlow;
        if (card.suit == "Diamond") return weaponGlow;
        return Color.white;
    }
}