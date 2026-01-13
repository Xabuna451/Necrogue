using UnityEngine;
using System;

public class EnemyHp : MonoBehaviour, IDamageable
{
    [SerializeField] EnemyCtrl ctx;

    [Header("HP State")]
    [SerializeField] int hp;               // 현재 HP

    public int Hp => hp;
    public int MaxHp { get; private set; }
    public int BaseMaxHp { get; private set; }   // ⭐ 기준값 (절대 변경 금지)

    bool dead;
    bool reserved;

    public bool Dead => dead;
    public bool Reserved => reserved;

    // 인터페이스
    public bool IsDead => dead;

    public event Action<EnemyHp, Faction> OnDied;

    NecromancerController necro;

    // ==================================================
    // Unity
    // ==================================================
    void Awake()
    {
        if (!ctx) ctx = GetComponent<EnemyCtrl>();
    }

    // ==================================================
    // Spawn / Init
    // ==================================================
    public void ResetForSpawn(int baseMaxHp)
    {
        dead = false;
        reserved = false;

        BaseMaxHp = Mathf.Max(1, baseMaxHp);
        MaxHp = BaseMaxHp;
        hp = MaxHp;
    }


    // ==================================================
    // Runtime Rebuild (네크로 퍼크 즉시 반영)
    // ==================================================
    public void SetMaxHp(int maxHp)
    {
        maxHp = Mathf.Max(1, maxHp);

        if (MaxHp == maxHp)
            return;

        MaxHp = maxHp;

        // ⭐ 현재 HP는 절대 회복하지 않고, 초과만 방지
        hp = Mathf.Min(hp, MaxHp);
    }

    // ==================================================
    // Damage / Death
    // ==================================================
    public void Damaged(int dmg)
    {
        if (dmg <= 0) return;
        if (dead) return;

        hp -= dmg;
        if (hp < 0) hp = 0;

        if (ctx && ctx.Animation) ctx.Animation.PlayHit();
        if (ctx) ctx.Retarget();

        if (hp == 0)
            Die();
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        var diedAs = ctx ? ctx.Faction : Faction.Enemy;

        OnDied?.Invoke(this, diedAs);

        if (diedAs == Faction.Ally)
        {
            ctx?.Despawn();
            return;
        }

        // FSM Dead 상태로 전환
        ctx?.StateMachine.SwitchState(EnemyStateType.Dead);
        reserved = false;
    }

    // ==================================================
    // Revive (예외적으로 회복 허용)
    // ==================================================
    public void Revive(int newMaxHp, bool fullHeal = true)
    {
        MaxHp = Mathf.Max(1, newMaxHp);

        hp = fullHeal
            ? MaxHp
            : Mathf.Clamp(hp, 1, MaxHp);

        dead = false;
        reserved = false;

        Debug.Log($"[EnemyHp] {name} revived with HP: {hp}/{MaxHp}");
    }

    // ==================================================
    // Corpse / Necro Hook
    // ==================================================
    public void OnDeathAnimationFinished()
    {
        if (!gameObject.activeInHierarchy) return;
        if (!ctx) return;

        if (necro == null)
        {
            var go = GameObject.FindGameObjectWithTag("Necromancer");
            if (go) necro = go.GetComponentInChildren<NecromancerController>();
        }

        if (necro == null)
        {
            ctx.Despawn();
            return;
        }

        reserved = necro.TryReserve(this);

        if (reserved)
        {
            necro.OnCorpseReady(this);
            return;
        }

        ctx.Despawn();
    }
}
