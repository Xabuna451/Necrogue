using System.Collections.Generic;
using UnityEngine;
using System;

using PlayerType = Necrogue.Player.Runtime.Player;
using Necrogue.Common.Data;
using Necrogue.RuntimeObject;
using Necrogue.Enemy.Data;
using Necrogue.Spawn;
using Necrogue.Player.Runtime;



namespace Necrogue.Game.Systems
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        // ============================================================================ 

        [Header("게임 상태")]
        [SerializeField] GameState state = GameState.Runtime;
        [SerializeField] RuntimeState runtimeState = RuntimeState.Playing;

        public GameState State => state;
        public RuntimeState RuntimeState => runtimeState;

        public bool IsPaused => state == GameState.Pause;

        public event Action<GameState, GameState> OnGameStateChanged;
        public event Action<RuntimeState, RuntimeState> OnRuntimeStateChanged;

        // ============================================================================

        [Header("게임 시간")]
        [SerializeField] GameClock gameClock;

        [Header("풀")]
        [SerializeField] ObjectPool pools;
        public PlayerType player;

        [Header("Enemy")]
        [SerializeField] EnemySpawnProfile spawnProfile;
        [SerializeField] EnemySpawner spawner;

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

            player = FindFirstObjectByType<PlayerType>();
            if (!spawner) spawner = FindFirstObjectByType<EnemySpawner>();
        }

        private void Start()
        {
            // Player 재확인
            if (player == null)
            {
                player = FindFirstObjectByType<PlayerType>();
                if (player != null)
                {
                    Debug.Log("[GameManager] Player found in Start.");
                }
                else
                {
                    Debug.LogError("[GameManager] Player still not found in Start!");
                    return;
                }
            }

            // Player 태그 확인
            if (player != null && !player.CompareTag("Ally"))
            {
                Debug.LogWarning($"[GameManager] Player tag is '{player.tag}', should be 'Ally'!");
            }

            // EnemyRegistry 설정 - Start에서 확실하게 처리
            var registry = EnemyRegistry.Instance;
            if (registry == null)
            {
                registry = FindFirstObjectByType<EnemyRegistry>();
                Debug.LogError("[GameManager] EnemyRegistry.Instance is null! Found via FindFirstObjectByType: " + (registry != null));
            }

            if (registry != null && player != null)
            {
                registry.SetPlayer(player.transform);
                Debug.Log("[GameManager] Registry initialized successfully.");
            }
            else
            {
                Debug.LogError($"[GameManager] Cannot set player - Registry: {registry != null}, Player: {player != null}");
            }

            ApplyTimeScale();
        }

        void Update()
        {
#if UNITY_EDITOR
            // ============ 디버그 키 =============

            if (Input.GetKeyDown(KeyCode.P)) gameClock.SkipTime(60f);
            if (Input.GetKeyDown(KeyCode.L)) player.Exp.DebugLevelUp();

            // ===================================
#endif
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

        void ApplyTimeScale()
        {
            bool freeze =
                state == GameState.Pause ||
                runtimeState == RuntimeState.LevelUp ||
                runtimeState == RuntimeState.GameOver;

            Time.timeScale = freeze ? 0f : 1f;
        }


        public void SetRuntimeState(RuntimeState next)
        {
            if (runtimeState == next) return;

            var prev = runtimeState;
            runtimeState = next;

            ApplyTimeScale();
            OnRuntimeStateChanged?.Invoke(prev, next);
        }


        public void SetPaused(bool paused)
        {
            var next = paused ? GameState.Pause : GameState.Runtime;
            if (state == next) return;

            var prev = state;
            state = next;

            ApplyTimeScale();
            OnGameStateChanged?.Invoke(prev, state);
        }



    }
}