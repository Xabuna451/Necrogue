using UnityEngine;

public class DeadStateSMB : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 물리 관련만 (시각과 무관한 것)
        var col = animator.GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }
}