using UnityEngine;

using Necrogue.Game.Systems;
using Necrogue.Enemy.Runtime;

namespace Necrogue.Spawn
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] EnemySpawnProfile prof;
        [SerializeField] Transform[] point;
        [SerializeField] GameClock clock;

        int alive;
        int eliteAlive;

        void Awake()
        {
            if (!clock)
                clock = FindFirstObjectByType<GameClock>();

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

            var stage = prof.GetStage(clock.Elapsed);
            if (stage == null) return;

            if (clock.Timer < stage.delay) return;
            clock.ResetTimer();

            Spawn(stage, pool);
        }

        void Spawn(EnemySpawnProfile.Stage stage, EnemyPool pool)
        {
            if (alive >= stage.maxAlive) return;

            // PickResult로 받기
            var pick = prof.Pick(stage, eliteAlive);
            if (pick.def == null) return;

            var p = point[Random.Range(0, point.Length)];
            var e = pool.GetEnemy(pick.def);
            if (!e) return;

            alive++;

            // 이벤트 중복 방지
            e.OnFactionChanged -= OnEnemyFactionChanged;
            e.OnFactionChanged += OnEnemyFactionChanged;

            e.OnDespawn -= OnEnemyDespawn;
            e.OnDespawn += OnEnemyDespawn;

            void OnEnemyDespawn(EnemyContext e)
            {
                if (e.IsElite) eliteAlive--;
            }

            // 위치 세팅
            e.transform.SetPositionAndRotation(p.position, p.rotation);

            // 엘리트 적용
            if (pick.IsElite)
            {
                // ApplyElite는 EnemyCtrl에 추가할 메서드 (아래 3번)
                e.ApplyElite(pick.elite);
                eliteAlive++;
            }
        }

        void OnEnemyFactionChanged(Faction prev, Faction next)
        {
            // “현재 적”이 더 이상 Enemy가 아니게 되면 alive 감소
            if (prev == Faction.Enemy && next != Faction.Enemy)
            {
                alive--;

                // 엘리트도 동시에 감소 (Enemy에서 빠져나가는 순간이 사실상 ‘사라진 것’)
                // 단, e 참조가 없으니 이 방식만으론 판단이 어렵다.
                // 그래서 엘리트 카운트는 아래 OnEnemyDespawn에서 최종 보정한다.
            }
        }

        void OnEnemyDespawn()
        {
            // 방어적 보정: 풀로 돌아가는 순간 확실히 정리
            // (alive는 이미 faction change로 줄었을 수도 있어서 clamp)
            alive = Mathf.Max(0, alive);

            // eliteAlive는 EnemyCtrl에 IsElite 같은 플래그가 있어야 정확히 줄일 수 있다.
            // 그래서 OnDespawn 이벤트를 EnemyCtrl 인스턴스별 람다로 연결하는 방식이 제일 정확하다.
        }
    }
}