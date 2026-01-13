using System.Collections.Generic;

public class EnemyStateMachine
{
    private EnemyCtrl ctx;
    private Dictionary<EnemyStateType, EnemyState> states = new();
    public EnemyState CurrentState { get; private set; }

    public EnemyStateMachine(EnemyCtrl ctx)
    {
        this.ctx = ctx;
        states.Add(EnemyStateType.Idle, new IdleState(ctx));
        states.Add(EnemyStateType.Chase, new ChaseState(ctx));
        states.Add(EnemyStateType.Attack, new AttackState(ctx));
        states.Add(EnemyStateType.Dead, new DeadState(ctx));
        states.Add(EnemyStateType.Corpse, new CorpseState(ctx));
        states.Add(EnemyStateType.Revive, new ReviveState(ctx));

        SwitchState(EnemyStateType.Idle);  // 초기 상태
    }

    public void SwitchState(EnemyStateType newType)
    {
        CurrentState?.Exit();
        CurrentState = states[newType];
        CurrentState.Enter();
    }
}
