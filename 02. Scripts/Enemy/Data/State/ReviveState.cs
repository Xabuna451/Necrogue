using UnityEngine;

public class ReviveState : EnemyState
{
    public ReviveState(EnemyCtrl ctx) : base(ctx) { }

    public override void Enter()
    {
        ctx.Animation?.PlayResurrection();

        ctx.SetFaction(Faction.Ally);
        ctx.SetCollider(true);
        //ctx.Visual.SetResurrect(true);  // 또는 직접 색상 설정
    }

    public override void Tick()
    {
        // 부활 애니메이션 끝나면 자동으로 Idle or Chase로
        // → Animator 이벤트로 처리 추천
    }
}