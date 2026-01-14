using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyContext ctx) : base(ctx) { }

    public override void Enter()
    {
        ctx.Animation?.SetMove(true);
        //Debug.Log($"[ChaseState] {ctx.name} ({ctx.Faction}) entered Chase state, target: {ctx.Target?.name}");
    }

    public override void Tick()
    {
        // 타겟 유효성 재확인
        if (ctx.Target == null || !ctx.IsValidTarget(ctx.Target))
        {
            ctx.StateMachine.SwitchState(EnemyStateType.Idle);
            return;
        }

        // 안전장치: def, ai, attack 확인
        if (ctx.def == null || ctx.def.ai == null || ctx.def.attack == null)
        {
            //Debug.LogError($"[ChaseState] {ctx.name} - Missing def, ai, or attack profile!");
            ctx.StateMachine.SwitchState(EnemyStateType.Idle);
            return;
        }

        float dist = Vector2.Distance(ctx.transform.position, ctx.Target.position);
        float attackRange = ctx.def.attack.attackRange;
        float chaseRange = ctx.def.ai.chaseRange;

        // 공격 범위 먼저 체크 (우선순위)
        if (dist <= attackRange)
        {
            //Debug.Log($"[ChaseState] {ctx.name} in attack range! dist={dist:F2}, range={attackRange:F2}");
            ctx.StateMachine.SwitchState(EnemyStateType.Attack);
            return;
        }

        // 추격 범위 벗어나면 Idle
        if (dist > chaseRange)
        {
            ctx.StateMachine.SwitchState(EnemyStateType.Idle);
            return;
        }

        // 추격 이동
        ctx.Move.MoveTo(ctx.Target.position);
    }

    public override void Exit()
    {
        ctx.Move.Stop();
    }
}