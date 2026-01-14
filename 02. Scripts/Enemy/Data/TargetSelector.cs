using UnityEngine;

public abstract class TargetSelector : ScriptableObject
{
    public abstract Transform SelectTarget(EnemyContext ctx);
}
