using UnityEngine;
using System;

public enum Faction
{
    Enemy,
    Ally,
    Corpse,
}

[Serializable]
public struct EnemyBaseStats
{
    public int maxHp;
    public float attack;
    public float moveSpeed;
}

[RequireComponent(typeof(EnemyHp))]
[RequireComponent(typeof(EnemyDirectMove))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyContactDamage))]
[RequireComponent(typeof(EnemyReward))]
public class EnemyCtrl : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector / Runtime refs
    // ─────────────────────────────────────────────
    public EnemyDefAsset def;

    [HideInInspector] public EnemyPool OwnerPool;
    [HideInInspector] public EnemyDefAsset OriginDef;

    // ─────────────────────────────────────────────
    // Base Stats (SO → Runtime Snapshot)
    // ─────────────────────────────────────────────
    public EnemyBaseStats BaseStats { get; private set; }

    // ─────────────────────────────────────────────
    // Cached components
    // ─────────────────────────────────────────────
    private EnemyHp hp;
    public EnemyHp Hp => hp;

    private EnemyDirectMove directMove;
    public EnemyDirectMove DirMove => directMove;

    public SpriteRenderer spriteRenderer;

    // private EnemyVisual enemyVisual;
    // public EnemyVisual Visual => enemyVisual;

    public EnemyStateMachine StateMachine { get; private set; }
    public IEnemyMove Move { get; private set; }
    public EnemyAnimation Animation { get; private set; }

    // ─────────────────────────────────────────────
    // Elite (Runtime)
    // ─────────────────────────────────────────────
    [SerializeField] private bool isEliteRuntime;
    public bool IsElite => isEliteRuntime;

    private Vector3 baseScale = Vector3.one;

    // ─────────────────────────────────────────────
    // Combat (Runtime only)
    // ─────────────────────────────────────────────
    [SerializeField] private float attackMul = 1f;
    public float AttackMul => attackMul;

    // ─────────────────────────────────────────────
    // Target / Layer
    // ─────────────────────────────────────────────
    public Transform Target { get; private set; }
    public LayerMask MyLayerMask => 1 << gameObject.layer;

    // ─────────────────────────────────────────────
    // Faction
    // ─────────────────────────────────────────────
    [SerializeField] private Faction faction = Faction.Enemy;
    public Faction Faction => faction;

    public event Action<Faction, Faction> OnFactionChanged;
    public event Action<EnemyCtrl> OnDespawn;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────
    private void Awake()
    {
        hp = GetComponent<EnemyHp>();
        Move = GetComponent<IEnemyMove>();
        Animation = GetComponent<EnemyAnimation>();
        directMove = GetComponent<EnemyDirectMove>();

        Animation?.Init(this);
        Move?.Init(this);
        // Visual?.Init(this);

        StateMachine = new EnemyStateMachine(this);

        baseScale = transform.localScale;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        var contact = GetComponentInChildren<EnemyContactDamage>();
        contact?.Init(this);

        Retarget();
    }

    Color lastColor;
    private void Update()
    {
        if (hp == null || hp.IsDead || StateMachine?.CurrentState == null) return;
        StateMachine.CurrentState.Tick();

        // if (spriteRenderer)
        // {
        //     if (spriteRenderer.color != lastColor)
        //     {
        //         Debug.LogError($"[COLOR CHANGED] {name} {lastColor} -> {spriteRenderer.color}\n{Environment.StackTrace}");
        //         lastColor = spriteRenderer.color;
        //     }
        // }
    }

    private void OnEnable()
    {
        EnemyRegistry.Instance?.Add(this);
    }

    private void OnDisable()
    {
        EnemyRegistry.Instance?.Remove(this);
    }

    // ─────────────────────────────────────────────
    // Reset / Spawn
    // ─────────────────────────────────────────────
    public void Reset()
    {
        Target = null;

        isEliteRuntime = false;
        transform.localScale = baseScale;

        // Faction 초기화
        faction = Faction.Enemy;
        tag = "Enemy";
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        // BaseStats 스냅샷 (딱 여기서만 세팅)
        if (def != null && def.stats != null)
        {
            BaseStats = new EnemyBaseStats
            {
                maxHp = Mathf.Max(1, def.stats.maxHp),
                attack = Mathf.Max(1f, def.attack.attackDamage),
                moveSpeed = def.stats.moveSpeed
            };
        }
        else
        {
            BaseStats = new EnemyBaseStats
            {
                maxHp = 1,
                attack = 1f,
                moveSpeed = 0f
            };
        }

        // Runtime 값 초기화
        attackMul = 1f;

        SetCollider(true);

        // Visual.ResetAll();

        Move?.Stop();
        hp?.ResetForSpawn(BaseStats.maxHp);
        Animation?.ResetForSpawn();

        StateMachine?.SwitchState(EnemyStateType.Idle);
        Retarget();
    }

    // ─────────────────────────────────────────────
    // Combat
    // ─────────────────────────────────────────────
    public void SetAttackMul(float mul)
    {
        attackMul = Mathf.Max(0.01f, mul);
    }

    // ─────────────────────────────────────────────
    // Elite
    // ─────────────────────────────────────────────
    public void ApplyElite(EnemyEliteProfile elite)
    {
        if (elite == null) return;

        isEliteRuntime = true;

        // 룩
        //Visual.SetElite(elite.color, elite.scaleMul);

        // 스케일
        transform.localScale = baseScale * Mathf.Max(0.1f, elite.scaleMul);

        // HP: BaseStats 기반으로 재설정
        int eliteMaxHp = Mathf.Max(1, Mathf.RoundToInt(BaseStats.maxHp * Mathf.Max(0.1f, elite.hpMul)));
        hp?.ResetForSpawn(eliteMaxHp);

        // 공격: 현재 구조는 "attackMul"이므로 배율로 반영
        attackMul = Mathf.Max(0.01f, elite.atkMul);

        // 이동속도: 네 Move 구현에 따라 달라서 2가지 중 하나 선택해야 함.
        // 1) BaseStats.moveSpeed만 바꾸는 건 의미 없을 수 있음(컴포넌트가 이미 값을 복사했을 수도).
        // 2) 가장 안전한 방법: Move/DirectMove에 SetSpeedMul 같은 API를 만든다(추천).
        // 지금 당장은 TODO로 두고, 속도는 나중에.
        // TODO: directMove?.SetSpeedMul(elite.moveMul);
    }


    // ─────────────────────────────────────────────
    // Faction
    // ─────────────────────────────────────────────
    public void SetFaction(Faction f)
    {
        if (faction == f) return;

        var prev = faction;
        faction = f;

        if (faction == Faction.Enemy)
        {
            tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            //   Visual.SetFaction(Faction.Enemy);
        }
        else if (faction == Faction.Ally)
        {
            tag = "Ally";
            gameObject.layer = LayerMask.NameToLayer("Ally");
            //  Visual.SetFaction(Faction.Ally);
        }
        else
        {
            tag = "Corpse";
            gameObject.layer = LayerMask.NameToLayer("Corpse");
            //Visual.SetFaction(Faction.Corpse);
        }

        if (EnemyRegistry.Instance != null)
        {
            EnemyRegistry.Instance.Remove(this);
            EnemyRegistry.Instance.Add(this);
        }

        OnFactionChanged?.Invoke(prev, faction);

        ClearTarget();
        Retarget();
        Move?.Stop();
    }

    // ─────────────────────────────────────────────
    // Death / Resurrection
    // ─────────────────────────────────────────────
    public void OnDeadAnimationFinished()
    {
        var necro = FindFirstObjectByType<NecromancerController>();
        if (necro && necro.TryReserve(Hp))
        {
            StateMachine.SwitchState(EnemyStateType.Corpse);
            necro.OnCorpseReady(Hp);
        }
        else
        {
            Despawn();
        }
    }

    public void OnResurrectionAnimationFinished()
    {
        Retarget();

        if (Target != null && IsValidTarget(Target))
            StateMachine.SwitchState(EnemyStateType.Chase);
        else
            StateMachine.SwitchState(EnemyStateType.Idle);
    }

    // ─────────────────────────────────────────────
    // Targeting
    // ─────────────────────────────────────────────
    public void Retarget()
    {
        var selector = def != null ? def.targetSelector : null;
        Target = selector != null ? selector.SelectTarget(this) : FindNearestTarget();
    }

    private Transform FindNearestTarget()
    {
        Transform best = null;
        float bestDist = float.MaxValue;

        if (Faction == Faction.Enemy)
        {
            var player = EnemyRegistry.Instance?.GetPlayer();
            if (player != null && IsValidTarget(player))
            {
                float dist = Vector2.Distance(transform.position, player.position);
                best = player;
                bestDist = dist;
            }
        }

        var opposites = EnemyRegistry.Instance?.GetOpposite(this);
        if (opposites != null)
        {
            foreach (var enemy in opposites)
            {
                if (enemy == null || !IsValidTarget(enemy.transform)) continue;

                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = enemy.transform;
                }
            }
        }

        return best;
    }

    public bool IsValidTarget(Transform t)
    {
        if (t == null || !t.gameObject.activeInHierarchy) return false;

        if (t.CompareTag("Ally") && !t.GetComponentInParent<EnemyCtrl>())
            return Faction == Faction.Enemy;

        var c = t.GetComponentInParent<EnemyCtrl>();
        if (c == null || c == this) return false;
        if (c.Faction == Faction.Corpse || c.Faction == this.Faction) return false;

        var thp = c.GetComponent<EnemyHp>();
        if (thp != null && thp.Dead) return false;

        return true;
    }

    public void ClearTarget()
    {
        Target = null;
    }

    // ─────────────────────────────────────────────
    // Pool
    // ─────────────────────────────────────────────
    public void Despawn()
    {
        EnemyRegistry.Instance?.Remove(this);
        OnDespawn?.Invoke(this);
        OwnerPool?.Return(this);
    }

    public void SetCollider(bool on)
    {
        // 물리 관련만 (시각과 무관한 것)
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = on;
    }
}