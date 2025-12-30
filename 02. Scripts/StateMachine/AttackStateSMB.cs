using UnityEngine;

public class AttackStateSMB : StateMachineBehaviour
{
    // 상태 진입 시
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyCtrl ctx = animator.GetComponent<EnemyCtrl>();
        if (ctx != null)
        {
            ctx.DirMove.isAttack = true;
            Debug.Log("공격 시작 " + ctx.DirMove.isAttack);
        }
    }

    // 상태 종료 시
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyCtrl ctx = animator.GetComponent<EnemyCtrl>();
        if (ctx != null)
        {
            ctx.DirMove.isAttack = false;
            Debug.Log("공격 종료 " + ctx.DirMove.isAttack);
        }
    }
}