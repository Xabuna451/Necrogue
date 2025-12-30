using UnityEngine;
using System;

public enum Faction
{
    Enemy,
    Ally,
    Corpse,
}

[RequireComponent(typeof(EnemyHp))]
[RequireComponent(typeof(EnemyDirectMove))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyContactDamage))]
[RequireComponent(typeof(EnemyChaseAI))]
[RequireComponent(typeof(EnemyReward))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyCtrl : MonoBehaviour
{
    public EnemyDefAsset def;
    private EnemyDirectMove directMove;
    public EnemyDirectMove DirMove => directMove;

    private EnemyHp hp;
    public EnemyHp Hp => hp;

    [HideInInspector] public EnemyPool OwnerPool;
    [HideInInspector] public EnemyDefAsset OriginDef;

    public IEnemyAttack Attack { get; private set; }
    public IEnemyMove Move { get; private set; }
    public IEnemyAI AI { get; private set; }
    public EnemyAnimation Animation { get; private set; }

    public Transform Target { get; private set; }
    public LayerMask myLayerMask => gameObject.layer;

    [SerializeField] private Faction faction = Faction.Enemy;

    public event Action<Faction, Faction> OnFactionChanged;
    public Faction Faction
    {
        get => faction;
        private set => faction = value;
    }

    public SpriteRenderer spriteRenderer;

    public event Action OnDespawn;

    private void Awake()
    {
        Attack = GetComponent<IEnemyAttack>();
        Move = GetComponent<IEnemyMove>();
        AI = GetComponent<IEnemyAI>();
        Animation = GetComponent<EnemyAnimation>();
        Animation?.Init(this);

        spriteRenderer = GetComponent<SpriteRenderer>();

        hp = GetComponent<EnemyHp>();
        directMove = GetComponent<EnemyDirectMove>();
    }

    private void Start()
    {
        Retarget();

        AI?.Init(this);
        Move?.Init(this);
        Attack?.Init(this);

        var contact = GetComponentInChildren<EnemyContactDamage>();
        contact?.Init(this);
    }

    private void Update()
    {
        if (hp != null && hp.Hp <= 0) return;
        AI?.Tick();
    }

    void OnEnable()
    {
        EnemyRegistry.Instance?.Add(this);
    }

    void OnDisable()
    {
        EnemyRegistry.Instance?.Remove(this);
    }

    public void SetFaction(Faction f)
    {
        if (faction == f) return;

        var prev = faction;
        faction = f;

        if (faction == Faction.Enemy) tag = "Enemy";
        else if (faction == Faction.Ally) tag = "Ally";
        else tag = "Corpse";

        EnemyRegistry.Instance?.Add(this);

        OnFactionChanged?.Invoke(prev, faction);

        ClearTarget();
        Retarget();
        Move?.Stop();
    }

    public void SetAttackMul(float mul)
    {
        if (Attack == null) return;
        var profile = def.attack;
        if (profile == null) return;

        int old = profile.attackDamage;
        profile.attackDamage = Mathf.Max(1, Mathf.RoundToInt(old * mul));
    }

    public void Retarget()
    {
        var selector = def.targetSelector;
        Target = selector != null ? selector.SelectTarget(this) : null;
    }

    public void Reset()
    {
        Target = null;
        SetFaction(Faction.Enemy);
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        var col = GetComponent<Collider2D>();
        if (col) col.enabled = true;

        if (spriteRenderer) spriteRenderer.color = Color.white;

        Move?.Stop();
        GetComponent<EnemyHp>()?.ResetForSpawn();
        Animation?.ResetForSpawn();

        Retarget();

    }

    public bool IsValidTarget(Transform t)
    {
        if (t == null) return false;
        if (!t.gameObject.activeInHierarchy) return false;

        if (t.CompareTag("Ally") && !t.GetComponentInParent<EnemyCtrl>()) // 플레이어
            return true;

        var c = t.GetComponentInParent<EnemyCtrl>();
        if (c == null) return false;

        if (c.Faction == Faction.Corpse) return false;
        if (c.Faction == this.Faction) return false;

        var hp = c.GetComponent<EnemyHp>();
        if (hp != null && hp.Dead) return false;

        return true;
    }


    public void ClearTarget()
    {
        Target = null;
    }


    public void Despawn()
    {
        EnemyRegistry.Instance?.Remove(this);
        OnDespawn?.Invoke();
        OwnerPool?.Return(this);
    }

}
