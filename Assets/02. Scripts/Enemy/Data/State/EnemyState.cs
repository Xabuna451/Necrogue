using UnityEngine;


using Necrogue.Enemy.Runtime;

namespace Necrogue.Enemy.Data.States
{

    public abstract class EnemyState
    {
        protected EnemyContext ctx;

        public EnemyState(EnemyContext ctx)
        {
            this.ctx = ctx;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Tick() { }
    }
}