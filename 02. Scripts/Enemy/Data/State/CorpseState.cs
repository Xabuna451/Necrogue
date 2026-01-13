using UnityEngine;

public class CorpseState : EnemyState
{
    public CorpseState(EnemyCtrl ctx) : base(ctx) { }

    public override void Enter()
    {
        // DeadState에서 이미 처리됐지만, 안전하게
        ctx.Move.Stop();
        ctx.Animation?.SetMove(false);

        ctx.SetCollider(false);
        ctx.SetFaction(Faction.Corpse);
    }


    public override void Tick()
    {
        // 아무것도 안 함 (네크로맨서가 직접 Revive 호출)
    }
}