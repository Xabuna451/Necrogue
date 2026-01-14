using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Target/Selector")]
public class NearestEnemySelector : TargetSelector
{
    // fallback 포함 가장 가까운 적 선택
    public override Transform SelectTarget(EnemyContext me)
    {
        Transform best = null;
        float bestSqr = float.PositiveInfinity;

        var reg = EnemyRegistry.Instance;
        if (reg == null) return null;

        var player = reg.GetPlayer();

        // Ally일 때는 플레이어를 타겟하지 않음
        if (me.Faction == Faction.Ally && player)
        {
            // 플레이어 제외
        }
        else if (player)
        {
            // Enemy일 때는 플레이어 우선
            Vector2 d = (Vector2)player.position - (Vector2)me.transform.position;
            float sqr = d.sqrMagnitude;
            bestSqr = sqr;
            best = player;
        }

        var list = reg.GetOpposite(me);
        if (list == null) return best;

        Vector2 mePos = me.transform.position;

        foreach (var e in list)
        {
            if (!e || !e.gameObject.activeInHierarchy || e.Faction == Faction.Corpse) continue;

            var hp = e.GetComponent<EnemyHp>();
            if (hp != null && hp.IsDead) continue;

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
