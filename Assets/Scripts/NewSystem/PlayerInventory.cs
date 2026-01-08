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
}