using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public PlayerModel Model { get; private set; }

    public void Initialize(PlayerModel model)
    {
        Model = model;
    }

    //public void PerformBarehandAttack(MonsterModel monster)
    //{
    //    monster.TakeDamage(Model.Stats.BaseDamage);
    //    Model.OnAttack?.Invoke();
    //}

    //public void PerformWeaponAttack(MonsterModel monster)
    //{
    //    Model.UseWeaponOn(monster);
    //}

    public void ConsumePotion(int healAmount)
    {
        Model.Heal(healAmount);
    }
}

