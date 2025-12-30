using UnityEngine;

public class ResurrectionStateSMB : StateMachineBehaviour
{
    private SpriteRenderer spriteRenderer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 부활 시 Layer 복구 (Enemy 또는 Ally)

        var ctrl = animator.GetComponent<EnemyCtrl>();
        animator.gameObject.layer = LayerMask.NameToLayer(ctrl.Faction == Faction.Enemy ? "Enemy" : "Ally");

        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.cyan; // 네크로맨서 색상

        // 콜라이더 활성화
        var col = animator.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        spriteRenderer.color = Color.cyan; // Ally 색상 유지
    }
}