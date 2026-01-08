using UnityEngine;

public enum CardType { Heart, Diamond, Spade, Clover, Joker }

[CreateAssetMenu(menuName ="Cards/CardSO")]
public class CardSO : ScriptableObject
{
    public string cardId;
    public CardType type;
    public int grade;
    public Sprite artwork;
    public string description;
}
