using UnityEngine;
using System.Collections.Generic;

using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Necro;
using Necrogue.Perk.Runtime;
using Necrogue.Weapon.Runtime;

namespace Necrogue.Player.Runtime
{
    /// <summary>
    /// 플레이어 캐릭터의 주요 컴포넌트들을 관리하는 클래스
    /// - 각 컴포넌트는 Player 참조를 받아 초기화
    /// - 스탯은 SO(Base) -> RuntimeStats(합성 결과)로 일괄 빌드 후 적용
    /// </summary>
    public class Player : MonoBehaviour
    {
        // ==================================================
        // [1] References (External)
        // ==================================================
        [Header("참조")]
        [SerializeField] private InputManager inputManager;

        [Header("베이스 스탯(SO)")]
        [SerializeField] public PlayerStatAsset Stats;

        [Header("기본 무기")]
        [SerializeField] private WeaponProfile defaultWeapon;

        // ==================================================
        // [2] Runtime Results (합성 결과)
        // ==================================================
        public PlayerRuntimeStats RuntimeStats { get; private set; } = new PlayerRuntimeStats();
        public NecroRuntimeParams NecroRuntime { get; private set; } = new NecroRuntimeParams();

        // ==================================================
        // [3] Stat 적용 대상 컴포넌트 (IStatAppliable)
        // ==================================================
        private readonly List<IStatAppliable> statAppliables = new();

        // ==================================================
        // [4] Public Accessors (기존 코드 호환용)
        // ==================================================
        public PlayerMovement Move => GetComponentInChildren<PlayerMovement>(true);
        public PlayerHp Hp => GetComponentInChildren<PlayerHp>(true);
        public PlayerExp Exp => GetComponentInChildren<PlayerExp>(true);
        public PlayerAttack Attack => GetComponentInChildren<PlayerAttack>(true);
        public PlayerHit Hit => GetComponentInChildren<PlayerHit>(true);
        public PlayerAnimation Animation => GetComponentInChildren<PlayerAnimation>(true);
        public PlayerWeapon Weapon => GetComponentInChildren<PlayerWeapon>(true);
        public PlayerNecroController Necro => GetComponentInChildren<PlayerNecroController>(true);
        public PerkSystem Perks => GetComponentInChildren<PerkSystem>(true);
        public PlayerUI UI => FindFirstObjectByType<PlayerUI>();

        public InputManager InputManager => inputManager;

        // ==================================================
        // [5] Unity Lifecycle
        // ==================================================
        private void Awake()
        {
            if (!inputManager) inputManager = FindFirstObjectByType<InputManager>();

            if (!Stats)
            {
                Debug.LogError("[Player] PlayerStatAsset(Stats) is missing.");
                return;
            }

            // IStatAppliable 자동 수집 (자식 포함)
            statAppliables.AddRange(GetComponentsInChildren<IStatAppliable>(true));

            InitComponents();
        }

        private void OnEnable()
        {
            var exp = Exp;
            if (exp != null)
                exp.OnLeveledUp += HandleLeveledUp;
        }

        private void OnDisable()
        {
            var exp = Exp;
            if (exp != null)
                exp.OnLeveledUp -= HandleLeveledUp;
        }

        void Start()
        {
            // 최초 1회 빌드
            RebuildStats();
            Hp?.ResetFull();
        }

        [System.Obsolete]
        // FixedUpdate는 이제 사용하지 않을 것 (각 컴포넌트가 스스로 관리하거나 GameManager에서 호출)
        void FixedUpdate()
        {
            // 이동 처리
            Move?.PhysicsMove();
            Hit.CheckHit();
        }


        // ==================================================
        // [6] Initialization
        // ==================================================
        private void InitComponents()
        {
            // Movement
            var movement = Move;
            if (movement) movement.Init(this);

            // Hp
            var hp = Hp;
            if (hp) hp.Init(this);

            // Attack
            var attack = Attack;
            if (attack) attack.Init(this);

            // Hit
            var hit = Hit;
            if (hit) hit.Init(this);

            // Exp
            var exp = Exp;
            if (exp) exp.Init(this);

            // Animation
            var anim = Animation;
            if (anim) anim.Init(this);

            // Weapon
            var weapon = Weapon;
            if (weapon)
            {
                weapon.Init(this);
                if (defaultWeapon) weapon.SetWeapon(defaultWeapon);
            }

            // Necro
            var necro = Necro;
            if (necro) necro.Init(this, inputManager);

            // PerkSystem
            var perks = Perks;
            if (perks) perks.Init(this);
        }

        // ==================================================
        // [7] Level Up
        // ==================================================
        private void HandleLeveledUp(int newLevel)
        {
            var perks = Perks;
            if (perks) perks.OnLevelUp();
        }

        // ==================================================
        // [8] Rebuild / Apply
        // ==================================================
        public void RebuildStats()
        {
            if (!Stats) return;

            // PerkSystem이 전체 재연산을 담당하는 구조라면 트리거만
            var perks = Perks;
            if (perks != null)
            {
                perks.RecalculateAll();
                return;
            }

            // PerkSystem이 없을 때만 직접 적용
            RuntimeStats.SetFromBase(Stats);
            ApplyRuntimeStats();
        }

        public void ApplyRuntimeStats()
        {
            // 이제 리스트 순회로 한 번에 적용
            foreach (var appliable in statAppliables)
            {
                appliable.ApplyStats(RuntimeStats, NecroRuntime);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                var necro = Necro;
                necro?.NecromancerLevelUp();
            }
        }
    }
}