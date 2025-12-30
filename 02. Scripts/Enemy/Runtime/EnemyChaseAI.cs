using UnityEngine;

public class EnemyChaseAI : MonoBehaviour, IEnemyAI
{
    EnemyCtrl ctx;
    EnemyAIProfile ai;
    EnemyDefAsset lastDef;

    public void Init(EnemyCtrl ctx)
    {
        this.ctx = ctx;
        lastDef = null;
        Refresh();
    }

    void Refresh()
    {
        if (ctx == null) return;
        if (ctx.def == null) { ai = null; return; }

        if (lastDef != ctx.def)
        {
            lastDef = ctx.def;
            ai = ctx.def.ai;
        }
    }

    public void Tick()
    {
        if (!ctx.IsValidTarget(ctx.Target))
        {
            ctx.ClearTarget();
            ctx.Retarget();
            if (!ctx.IsValidTarget(ctx.Target))
            {
                ctx.Move.Stop();
                return;
            }
        }

        Refresh();

        if (ctx == null) { Debug.Log("tick stop ctx null"); return; }
        if (ai == null) { Debug.Log("tick stop ai null " + (ctx.def ? ctx.def.name : "no def")); return; }
        if (ctx.Move == null) { Debug.Log("tick stop move null"); return; }
        if (ctx.Faction == Faction.Corpse) { Debug.Log("tick stop corpse"); return; }
        if (ctx == null || ai == null || ctx.Move == null) return;
        if (ctx.Faction == Faction.Corpse) return;

        if (ctx.Target == null)
        {
            ctx.Retarget();
            if (ctx.Target == null) { ctx.Move.Stop(); return; }
        }

        float dist = Vector2.Distance(ctx.transform.position, ctx.Target.position);

        if (dist > ai.chaseRange)
        {
            ctx.Move.Stop();
            return;
        }

        if (dist <= ai.stopRange)
        {
            ctx.Move.Stop();
            ctx.Attack?.Tick();
            return;
        }

        ctx.Move.MoveTo(ctx.Target.position);
    }
}
