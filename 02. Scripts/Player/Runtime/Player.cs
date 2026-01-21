using UnityEngine;

using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Necro;      // NecroRuntimeParams 쓴다면 필요
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

        // ==================================================
        // [2] Components (Runtime)
        // ==================================================
        [Header("플레이어 컴포넌트")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerHp playerHp;
        [SerializeField] private PlayerExp playerExp;
        [SerializeField] private PlayerAttack playerAttack;
        [SerializeField] private PlayerHit playerHit;
        [SerializeField] private PlayerAnimation playerAnimation;
        [SerializeField] private PlayerWeapon playerWeapon;
        [SerializeField] private PlayerUI ui;

        [Header("네크로맨서")]
        [SerializeField] private PlayerNecroController necroController;

        [Header("기본 무기")]
        [SerializeField] private WeaponProfile defaultWeapon;

        [Header("특전")]
        [SerializeField] private PerkSystem perkSystem;

        // ==================================================
        // [3] Public Accessors
        // ==================================================
        public PlayerMovement Move => playerMovement;
        public PlayerHp Hp => playerHp;
        public PlayerExp Exp => playerExp;
        public PlayerAttack Attack => playerAttack;
        public PlayerHit Hit => playerHit;
        public PlayerWeapon Weapon => playerWeapon;
        public PlayerAnimation Animation => playerAnimation;
        public PlayerNecroController Necro => necroController;
        public PerkSystem Perks => perkSystem;
        public PlayerUI UI => ui;

        public InputManager InputManager => inputManager;

        // ==================================================
        // [4] Runtime Results (합성 결과)
        // ==================================================
        public PlayerRuntimeStats RuntimeStats { get; private set; } = new PlayerRuntimeStats();

        // PerkSystem이 Necro까지 재연산하는 구조라면 Player도 들고 있어야 함
        public NecroRuntimeParams NecroRuntime { get; private set; } = new NecroRuntimeParams();

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

            InitComponents();
        }

        private void OnEnable()
        {
            if (playerExp != null)
                playerExp.OnLeveledUp += HandleLeveledUp;
        }

        private void OnDisable()
        {
            if (playerExp != null)
                playerExp.OnLeveledUp -= HandleLeveledUp;
        }

        void Start()
        {
            // 최초 1회 빌드
            RebuildStats();
            Hp.ResetFull();
        }

        [System.Obsolete]
        private void FixedUpdate()
        {
            playerMovement?.PhysicsMove();
            playerHit?.CheckHit();
        }

        // ==================================================
        // [6] Initialization
        // ==================================================
        private void InitComponents()
        {
            // 전부 Init(this)로 통일 (Movement만 Init()였던 부분 수정)
            if (playerMovement) playerMovement.Init(this);
            if (playerHp) playerHp.Init(this);
            if (playerAttack) playerAttack.Init(this);
            if (playerHit) playerHit.Init(this);
            if (playerExp) playerExp.Init(this);
            if (playerAnimation) playerAnimation.Init(this);

            if (playerWeapon)
            {
                playerWeapon.Init(this);
                if (defaultWeapon) playerWeapon.SetWeapon(defaultWeapon);
            }

            if (necroController)
                necroController.Init(this, inputManager);

            if (perkSystem)
                perkSystem.Init(this);
        }

        // ==================================================
        // [7] Level Up
        // ==================================================
        private void HandleLeveledUp(int newLevel)
        {
            if (perkSystem) perkSystem.OnLevelUp();
        }

        // ==================================================
        // [8] Rebuild / Apply
        // ==================================================
        public void RebuildStats()
        {
            if (!Stats) return;

            // PerkSystem이 “전체 재연산 + ApplyRuntimeStats()까지” 담당하는 구조면
            // Player는 그냥 트리거만 한다.
            if (perkSystem != null)
            {
                perkSystem.RecalculateAll();
                return;
            }

            // (PerkSystem이 없다면) Base -> Runtime 복사 후 적용만
            RuntimeStats.SetFromBase(Stats);
            ApplyRuntimeStats();
        }

        public void ApplyRuntimeStats()
        {
            if (playerMovement) playerMovement.ApplyStats(RuntimeStats);
            if (playerAttack) playerAttack.ApplyStats(RuntimeStats);
            if (playerHp) playerHp.ApplyStats(RuntimeStats);

            // NecroRuntime을 실제 네크로 시스템에 반영하는 함수는
            // PlayerNecroController에 ApplyRuntime(NecroRuntimeParams)로 붙일 예정
            if (necroController) necroController.ApplyRuntime(NecroRuntime);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Necro.NecromancerLevelUp();
            }
        }
    }


}
