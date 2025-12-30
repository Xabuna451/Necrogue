using UnityEngine;

public class DeadStateSMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var ctrl = animator.GetComponent<EnemyCtrl>();
        var hp = animator.GetComponent<EnemyHp>();
        var col = animator.GetComponent<Collider2D>();

        if (ctrl != null)
        {
            ctrl.Move?.Stop();
            ctrl.Animation?.SetMove(false);
        }

        // 콜라이더 비활성화
        if (col != null)
            col.enabled = false;

        // Layer를 Corpse로 변경 (Raycast 타겟팅용, 시체 상태일 때만)
        if (ctrl != null && ctrl.Faction == Faction.Corpse)
            animator.gameObject.layer = LayerMask.NameToLayer("Corpse");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var ctrl = animator.GetComponent<EnemyCtrl>();
        if (ctrl != null && ctrl.Faction == Faction.Corpse)
            animator.GetComponent<EnemyHp>()?.OnDeathAnimationFinished();
    }
}