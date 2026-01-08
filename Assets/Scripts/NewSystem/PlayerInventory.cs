using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Stack<CardSO> potionStack = new Stack<CardSO>();
    private Stack<WeaponInstance> weaponStack = new Stack<WeaponInstance>();
    public static PlayerInventory instance { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddPotion(CardSO potion)
    {
        potionStack.Push(potion);
        Debug.Log("Added potion, grade : " + potion.grade + ", total potion : " + potionStack.Count);
    }

    public CardSO UsePotion()
    {
        if(potionStack.Count == 0 )
        {
            return null;
        }
        var potion = potionStack.Pop();
        Debug.Log("Used potion, grade : " + potion.grade + ", remaining potion : " + potionStack.Count);
        return potion;
    }

    public void PickupWeapon(CardSO weaponCard)
    {
        var weapon = new WeaponInstance(weaponCard.grade, "pickup");
        weaponStack.Push(weapon);
        Debug.Log("Picked up weapon, grade : " + weapon.grade + ", total weapons : " + weaponStack.Count);
    }

    public void AddWeaponFromEnemy(int EnemyGrade)
    {
        var weapon = new WeaponInstance(EnemyGrade, "enemy");
        weaponStack.Push(weapon);
        Debug.Log("Added weapon from enemy, grade : " + weapon.grade + ", total weapons : " + weaponStack.Count);
    }

    public void DiscardWeapon()
    {
        weaponStack.Clear();
        Debug.Log("All weapons discarded, total weapons : " + weaponStack.Count);
    }

    public WeaponInstance GetCurrectWeapon()
    {
        return weaponStack.Count > 0 ? weaponStack.Peek() : null;
    }
}