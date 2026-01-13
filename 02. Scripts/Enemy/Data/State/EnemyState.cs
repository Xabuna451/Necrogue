using UnityEngine;


public abstract class EnemyState
{
    protected EnemyCtrl ctx;

    public EnemyState(EnemyCtrl ctx)
    {
        this.ctx = ctx;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
}