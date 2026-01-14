using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerController : MonoBehaviour
{
    [SerializeField] PlayerStatAsset stat;

    // Perk 상태(이벤트)
    [SerializeField] NecroPerkState perk;

    [Header("Elite Undead?")]
    [SerializeField] private bool isEliteUndead = false;

    readonly List<EnemyContext> undead = new();
    readonly HashSet<EnemyHp> reserved = new();
    readonly HashSet<EnemyHp> reanim = new();

    int BaseMax => stat && stat.necromaner != null ? stat.necromaner.maxCount : 0;
    int Slots => undead.Count + reserved.Count + reanim.Count;

    int Max => BaseMax + (perk ? perk.AllyCapBonus : 0);
    public bool HasSlot => Slots < Max;

    void Awake()
    {
        if (!perk) perk = GetComponent<NecroPerkState>();

        if (perk) perk.OnChanged += RefreshUndeadStats;

        Debug.Log($"[NecromancerController] Awake on {gameObject.name} active={gameObject.activeInHierarchy}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Log("[NecromancerController] P pressed");
    }

    void OnDestroy()
    {
        if (perk) perk.OnChanged -= RefreshUndeadStats;
    }

    // ============================ Perk 적용(갱신) ============================
    void RefreshUndeadStats()
    {
        for (int i = undead.Count - 1; i >= 0; i--)
        {
            var ctrl = undead[i];
            if (!ctrl) { undead.RemoveAt(i); continue; }
            if (ctrl.Faction != Faction.Ally) continue;

            var ehp = ctrl.Hp;
            if (ehp == null || ehp.Dead) continue; // 갱신 중 Revive로 dead=false 되는 사고 방지

            ApplyUndeadAttack(ctrl);
        }
    }

    void ApplyUndeadHp(EnemyContext ctrl, bool fullHeal)
    {
        var prof = stat ? stat.necromaner : null;
        if (!ctrl || !ctrl.def || prof == null) return;
        if (!perk) return;

        var hp = ctrl.Hp;
        if (hp == null || ctrl.def.stats == null) return;

        int baseMaxHp = Mathf.Max(1, ctrl.def.stats.maxHp);

        int newMaxHp = NecroUndeadStatFormula.ComputeMaxHp(
            baseMaxHp,
            prof.hpMul,
            perk.AllyHpAdd,
            perk.AllyHpMul
        );

        hp.Revive(newMaxHp, fullHeal);
    }

    void ApplyUndeadAttack(EnemyContext ctrl)
    {
        var prof = stat ? stat.necromaner : null;
        if (!ctrl || !ctrl.def || prof == null) return;
        if (!perk) return;
        if (ctrl.def.attack == null) return;

        float baseAtk = ctrl.def.attack.attackDamage;

        float finalMul = NecroUndeadStatFormula.ComputeAttackMul(
            baseAtk,
            prof.attackMul,
            perk.AllyDamageAdd,
            perk.AllyDamageMul
        );

        ctrl.SetAttackMul(finalMul);
    }

    // ============================ 부활 흐름 ============================
    public bool TryReserve(EnemyHp hp)
    {
        if (!hp) return false;

        var prof = stat ? stat.necromaner : null;
        if (prof == null) return false;

        var enemy = hp.GetComponent<EnemyContext>();
        if (!enemy || !enemy.def) return false;

        if (enemy.Faction != Faction.Corpse) return false;
        if (reserved.Contains(hp) || reanim.Contains(hp)) return false;

        bool boss = enemy.def.boss != null;
        if (boss && !prof.bossOK) return false;

        if (Random.value >= prof.reviveChance) return false;
        Debug.Log($"[Necro] Slots={Slots}, Max={Max} (BaseMax={BaseMax}, CapBonus={(perk ? perk.AllyCapBonus : 0)})");
        if (!HasSlot) return false;

        reserved.Add(hp);
        return true;
    }

    public void OnCorpseReady(EnemyHp hp)
    {
        if (!hp) return;
        if (!reserved.Contains(hp)) return;
        if (reanim.Contains(hp)) return;

        reserved.Remove(hp);
        reanim.Add(hp);

        var ctrl = hp.GetComponent<EnemyContext>();
        if (ctrl) ctrl.StateMachine.SwitchState(EnemyStateType.Corpse);

        StartCoroutine(Reanimate(hp));
    }

    IEnumerator Reanimate(EnemyHp hp)
    {
        var ctrl = hp.GetComponent<EnemyContext>();
        if (!ctrl) { reanim.Remove(hp); yield break; }

        var prof = stat ? stat.necromaner : null;
        if (prof == null) { reanim.Remove(hp); yield break; }

        yield return new WaitForSeconds(prof.reviveDelay);

        if (!hp.gameObject.activeInHierarchy) { reanim.Remove(hp); yield break; }

        ctrl.StateMachine.SwitchState(EnemyStateType.Revive);

        // 부활은 풀힐 + 공격 적용
        ApplyUndeadHp(ctrl, fullHeal: true);
        ApplyUndeadAttack(ctrl);

        if (!undead.Contains(ctrl))
        {
            undead.Add(ctrl);
            hp.OnDied -= OnDied;
            hp.OnDied += OnDied;
        }

        reanim.Remove(hp);
    }

    void OnDied(EnemyHp hp, Faction diedAs)
    {
        if (diedAs != Faction.Ally) return;

        var ctrl = hp.GetComponent<EnemyContext>();
        if (ctrl) undead.Remove(ctrl);

        hp.OnDied -= OnDied;
    }

    // API
    public void EliteUndead(bool on)
    {
        isEliteUndead = on;
    }
}
