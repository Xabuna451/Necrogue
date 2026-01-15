using UnityEngine;


using Necrogue.Enemy.Runtime;

namespace Necrogue.Enemy.Data
{

    public abstract class TargetSelector : ScriptableObject
    {
        public abstract Transform SelectTarget(EnemyContext ctx);
    }
}