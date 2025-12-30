using UnityEngine;
using System;

public class EnemyHp : MonoBehaviour
{
    [SerializeField] EnemyCtrl ctx;
    [SerializeField] int hp;

    public int Hp => hp;
    public int MaxHp { get; private set; }

    bool dead;
    bool reserved;

    public bool Dead => dead;
    public bool Reserved => reserved;

    public event Action<EnemyHp, Faction> OnDied;

    NecromancerController necro;

    void Awake()
    {
        if (!ctx) ctx = GetComponent<EnemyCtrl>();
    }

    public void ResetForSpawn()
    {
        dead = false;
        reserved = false;

        int max = 1;
        if (ctx && ctx.def && ctx.def.stats) max = ctx.def.stats.maxHp;

        MaxHp = Mathf.Max(1, max);
        hp = MaxHp;
    }

    public void Damaged(int dmg)
    {
        if (dmg <= 0) return;
        if (dead) return;

        hp -= dmg;
        if (hp < 0) hp = 0;

        if (ctx && ctx.Animation) ctx.Animation.PlayHit();
        if (ctx) ctx.Retarget();

        if (hp == 0) Die();
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

        ctx?.Animation?.PlayDead();
        ctx?.SetFaction(Faction.Corpse);

        reserved = false;
    }

    public void Revive(int newMaxHp, bool fullHeal = true)
    {
        MaxHp = Mathf.Max(1, newMaxHp);
        hp = fullHeal ? MaxHp : Mathf.Clamp(hp, 1, MaxHp);

        dead = false;
        reserved = false;
    }

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
