using UnityEngine;

public class CorpseState : EnemyState
{
    public CorpseState(EnemyContext ctx) : base(ctx) { }

    public override void Enter()
    {
        ctx.Move.Stop();
        ctx.Animation?.SetMove(false);

        ctx.SetCollider(false);
        ctx.ChangeFaction(Faction.Corpse, stopMove: false, retarget: false);
    }


    public override void Tick()
    {
        // 아무것도 안 함 (네크로맨서가 직접 Revive 호출)
    }
}