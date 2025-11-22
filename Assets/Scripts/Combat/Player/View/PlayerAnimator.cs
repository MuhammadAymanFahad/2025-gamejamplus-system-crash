using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayHurt()
    {
        animator.SetTrigger("Hurt");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Death");
    }
}


