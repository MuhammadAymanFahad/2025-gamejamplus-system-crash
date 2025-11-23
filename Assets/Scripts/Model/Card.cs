using UnityEngine;

public class Card
{
    public string title => data.name;
    public string description => data.Description;
    public Sprite card_image => data.card_image;
    public int Durability {get ; private set;} 
    private readonly CardData data;
    public Card(CardData cardData)
    {
        data = cardData;
        Durability = cardData.Durability;
    }    
}
