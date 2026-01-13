using UnityEngine;

public class IdleState : EnemyState
{
    public IdleState(EnemyCtrl ctx) : base(ctx) { }

    public override void Enter()
    {
        ctx.Move.Stop();
        ctx.Animation.SetMove(false);
        ctx.ClearTarget();
        //Debug.Log($"[IdleState] {ctx.name} ({ctx.Faction}) entered Idle");
    }

    public override void Tick()
    {
        // 안전장치: def 확인
        if (ctx.def == null)
        {
            //Debug.LogWarning($"[IdleState] {ctx.name} - def is null, cannot retarget");
            return;
        }

        ctx.Retarget();

        if (ctx.Target != null && ctx.IsValidTarget(ctx.Target))
        {
            //Debug.Log($"[IdleState] {ctx.name} ({ctx.Faction}) found target: {ctx.Target.name}, switching to Chase");
            ctx.StateMachine.SwitchState(EnemyStateType.Chase);
        }
    }
}