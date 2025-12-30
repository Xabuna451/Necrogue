using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("참조")]
    [SerializeField] ObjectPool pools;
    public Player player;

    [Header("Enemy")]
    [SerializeField] EnemySpawnProfile spawnProfile;

    [Header("Player Bullet")]
    [SerializeField] PlayerBullet bulletPrefab;
    [SerializeField] int bulletInitialSize = 200;

    [Header("Reward Prefabs")]
    [SerializeField] Exp expPrefab;
    [SerializeField] Gold goldPrefab;
    [SerializeField] int rewardInitialEach = 50;

    public ObjectPool Pools => pools;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!pools)
        {
            Debug.LogError("[GameManager] pools(ObjectPool) 미할당");
            return;
        }

        var defs = CollectDefs(spawnProfile);

        pools.Init(
            enemyDefs: defs, enemyEach: 20,
            bulletPrefab: bulletPrefab, bulletSize: bulletInitialSize,
            expPrefab: expPrefab, goldPrefab: goldPrefab, rewardEach: rewardInitialEach
        );

        player = FindFirstObjectByType<Player>();
        EnemyRegistry.Instance.SetPlayer(player.transform);
    }

    EnemyDefAsset[] CollectDefs(EnemySpawnProfile prof)
    {
        if (!prof || prof.stages == null)
        {
            Debug.LogError("[GameManager] spawnProfile 비어 있음");
            return new EnemyDefAsset[0];
        }

        var set = new HashSet<EnemyDefAsset>();

        foreach (var stage in prof.stages)
        {
            if (stage == null || stage.table == null) continue;
            foreach (var e in stage.table)
                if (e.def) set.Add(e.def);
        }

        return new List<EnemyDefAsset>(set).ToArray();
    }
}
