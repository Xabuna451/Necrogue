using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemySpawnProfile prof;
    [SerializeField] Transform[] point;

    int alive;
    float elapsed;
    float timer;

    void Awake()
    {
        var all = GetComponentsInChildren<Transform>();
        point = new Transform[all.Length - 1];
        for (int i = 1; i < all.Length; i++)
            point[i - 1] = all[i];
    }

    void Update()
    {
        if (!prof) return;

        var pool = GameManager.Instance?.Pools?.Enemies;
        if (!pool) return;

        if (point == null || point.Length == 0) return;

        elapsed += Time.deltaTime;

        var stage = prof.GetStage(elapsed);
        if (stage == null) return;

        timer += Time.deltaTime;
        if (timer < stage.delay) return;
        timer = 0f;

        Spawn(stage, pool);
    }

    void Spawn(EnemySpawnProfile.Stage stage, EnemyPool pool)
    {
        if (alive >= stage.maxAlive) return;

        var def = prof.Pick(stage);
        if (!def) return;

        var p = point[Random.Range(0, point.Length)];
        var e = pool.GetEnemy(def);
        if (!e) return;

        alive++;

        e.OnFactionChanged -= OnEnemyFactionChanged;
        e.OnFactionChanged += OnEnemyFactionChanged;

        e.transform.SetPositionAndRotation(p.position, p.rotation);
    }

    void OnEnemyFactionChanged(Faction prev, Faction next)
    {
        if (prev == Faction.Enemy && next != Faction.Enemy)
            alive--;
    }
}
