using UnityEngine;

[CreateAssetMenu(fileName = "RoomCard_", menuName = "Scoundrel/Room Card Data")]
public class RoomCardData : ScriptableObject
{
    public CardSuit suit;
    public int rank; // 2-10, 11=J, 12=Q, 13=K, 14=Ace
    public Sprite cardSprite;

    public int Grade => rank;
    public bool IsMonster => suit == CardSuit.Clover || suit == CardSuit.Shovel;
    public bool IsPotion => suit == CardSuit.Heart;
    public bool IsWeapon => suit == CardSuit.Diamond;
    public bool IsBoss => rank == 14 && IsMonster;
}

public enum CardSuit { Clover, Shovel, Heart, Diamond }