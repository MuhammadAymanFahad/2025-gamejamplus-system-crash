using System;
using UnityEngine;

public class PlayerModel
{
    public PlayerStats Stats { get; private set; }
    public Weapon Weapon { get; private set; }

    public event Action OnHealthChanged;
    public event Action OnWeaponChanged;
    public event Action OnAttack;
    public event Action OnDamaged;
    public event Action OnDeath;

    public PlayerModel(PlayerStats stats)
    {
        Stats = stats;
    }

    public void TakeDamage(int amount)
    {
        Stats.CurrentHP -= amount;
        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke();

        if (Stats.CurrentHP <= 0)
            OnDeath?.Invoke();
    }

    public void Heal(int amount)
    {
        Stats.CurrentHP = Mathf.Min(Stats.MaxHP, Stats.CurrentHP + amount);
        OnHealthChanged?.Invoke();
    }

    //public void UseWeaponOn(MonsterModel monster)
    //{
    //    OnAttack?.Invoke();
    //    monster.TakeDamage(Weapon.Grade);
    //}

    public void SetWeapon(Weapon weapon)
    {
        Weapon = weapon;
        OnWeaponChanged?.Invoke();
    }
}

