using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [field : SerializeField] public string Description;
    [field : SerializeField] public int Durability;
    [field : SerializeField] public Sprite card_image;
}
