using UnityEngine;

public class DeadState : EnemyState
{
    public DeadState(EnemyContext ctx) : base(ctx) { }

    public override void Enter()
    {
        ctx.Move.Stop();
        ctx.Animation?.SetMove(false);
        ctx.Animation?.PlayDead();

        ctx.ChangeFaction(Faction.Corpse, stopMove: true, retarget: false);
        ctx.SetCollider(false);
    }

    public override void Exit()
    {
        // Dead에서 나갈 때 (e.g., Revive로) 복구
        // var col = ctx.GetComponent<Collider2D>();
        // if (col) col.enabled = true;
        // 여기서 하니까 Dead 상태에서 콜라이더가 켜져서 문제 발생
    }

    public override void Tick()
    {
        // 애니메이션 끝나면 자동으로 Corpse 상태로 넘어가도록
        // → Animator에서 Dead 애니 끝날 때 이벤트로 OnDeathAnimationFinished() 호출
        // 여기선 아무것도 안 해도 됨
    }
}