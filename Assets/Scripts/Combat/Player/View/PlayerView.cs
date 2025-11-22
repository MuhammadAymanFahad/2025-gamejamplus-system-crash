using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public PlayerModel Model;
    public PlayerAnimator Animator;
    public PlayerUI UI;

    void Start()
    {
        Model.OnHealthChanged += UpdateHP;
        Model.OnAttack += PlayAttackAnimation;
        Model.OnDamaged += PlayHurtEffect;
        Model.OnDeath += PlayDeathAnimation;
    }

    void UpdateHP()
    {
        //UI.UpdateHPBar(Model.Stats.CurrentHP, Model.Stats.MaxHP);
    }

    void PlayAttackAnimation()
    {
        Animator.PlayAttack();
    }

    void PlayHurtEffect()
    {
        Animator.PlayHurt();
    }

    void PlayDeathAnimation()
    {
        Animator.PlayDeath();
    }
}
