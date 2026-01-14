using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    EnemyContext ctx;
    EnemyHp hp;

    bool droppedThisLife;

    [Header("Scatter")]
    [SerializeField] float scatterRadius = 0.4f;

    void Awake()
    {
        ctx = GetComponent<EnemyContext>();
        hp = GetComponent<EnemyHp>();

        if (!ctx) Debug.LogError("[EnemyReward] EnemyCtrl 없음");
        if (!hp) Debug.LogError("[EnemyReward] EnemyHp 없음");
    }

    void OnEnable()
    {
        droppedThisLife = false;

        if (hp != null)
        {
            hp.OnDied -= OnDied;
            hp.OnDied += OnDied;
        }
    }

    void OnDisable()
    {
        if (hp != null)
            hp.OnDied -= OnDied;
    }

    void OnDied(EnemyHp hp, Faction diedAs)
    {
        if (diedAs != Faction.Enemy) return;
        DropReward();
    }

    public void DropReward()
    {
        if (!ctx || !ctx.def) return;

        var rewardAsset = ctx.def.reward;
        if (!rewardAsset) return;

        var rewardPool = GameManager.Instance?.Pools?.Rewards;
        if (!rewardPool) return;

        bool isElite = ctx.def.isElite;

        int exp = rewardAsset.RollExp(isElite);
        int gold = rewardAsset.RollGold(isElite);

        Vector3 basePos = transform.position;

        if (exp > 0)
        {
            var e = rewardPool.GetExp();
            if (e)
            {
                e.Init(exp);
                e.transform.position = basePos + (Vector3)(Random.insideUnitCircle * scatterRadius);
            }
        }

        if (gold > 0)
        {
            var g = rewardPool.GetGold();
            if (g)
            {
                g.Init(gold);
                g.transform.position = basePos + (Vector3)(Random.insideUnitCircle * scatterRadius);
            }
        }
    }
}