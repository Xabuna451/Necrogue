using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private EnemyCtrl ctx;
    private Animator animator;

    public void Init(EnemyCtrl ctx)
    {
        this.ctx = ctx;
        animator = GetComponent<Animator>();
    }

    // ===== 외부에서 호출되는 API =====

    public void SetMove(bool moving)
    {
        if (animator) animator.SetBool("Move", moving);
    }

    public void PlayAttack()
    {
        if (animator) animator.SetTrigger("Attack");
    }

    public void PlayHit()
    {
        if (animator) animator.SetTrigger("Hit");
    }

    public void PlayDead()
    {
        //Debug.Log("tq");
        if (animator) animator.SetTrigger("Dead");
    }

    public void PlayResurrection()
    {
        if (animator) animator.SetTrigger("Resurrection");
    }

    public void ResetForSpawn()
    {
        if (animator)
        {
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Dead");
            animator.ResetTrigger("Resurrection");
            animator.SetBool("Move", false);
        }
    }
}
