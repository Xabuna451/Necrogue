using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Target/Selector")]
public class NearestEnemySelector : TargetSelector
{
    // fallback 포함 가장 가까운 적 선택
    public override Transform SelectTarget(EnemyCtrl me)
    {
        Transform best = null;
        float bestSqr = float.PositiveInfinity;

        var player = GameManager.Instance ? GameManager.Instance.player : null;
        if (player)
        {
            Vector2 d = (Vector2)player.transform.position - (Vector2)me.transform.position;
            float sqr = d.sqrMagnitude;
            bestSqr = sqr;
            best = player.transform;
        }

        var reg = EnemyRegistry.Instance;
        if (reg == null) return best;

        var list = reg.GetOpposite(me);
        if (list == null) return best;

        Vector2 mePos = me.transform.position;

        foreach (var e in list)
        {
            if (!e) continue;
            if (!e.gameObject.activeInHierarchy) continue;
            if (e.Faction == Faction.Corpse) continue;

            var hp = e.GetComponent<EnemyHp>();
            if (hp != null && hp.Dead) continue;

            Vector2 d = (Vector2)e.transform.position - mePos;
            float sqr = d.sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = e.transform;
            }
        }

        return best;
    }

}
