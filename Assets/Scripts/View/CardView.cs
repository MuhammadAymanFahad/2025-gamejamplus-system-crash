using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text durability;
    [SerializeField] private TMP_Text description;
    [SerializeField] private SpriteRenderer card_image;
    [SerializeField] private GameObject wrapper;
    public Card Card {get; private set;}
    public void Setup (Card card)
    {
        Card = card;

        title.text = card.title;
        description.text = card.description;
        durability.text = card.Durability.ToString();
        card_image.sprite = card.card_image;
        card_image.size = new Vector2(0.4f, 0.4f);
    }

    private void OnMouseEnter()
    {
        wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, -2, 0);
        CardViewHoverSystem.Instance.Show(Card, pos);
    }

    private void OnMouseExit()
    {
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }

}
