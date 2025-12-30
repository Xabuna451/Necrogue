using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerController : MonoBehaviour
{
    [SerializeField] PlayerStatAsset stat;

    readonly List<EnemyCtrl> undead = new();
    readonly HashSet<EnemyHp> reserved = new();
    readonly HashSet<EnemyHp> reanim = new();

    int Max => stat && stat.necromaner != null ? stat.necromaner.maxCount : 0;

    int Slots => undead.Count + reserved.Count + reanim.Count;

    public bool HasSlot => Slots < Max;

    public bool TryReserve(EnemyHp hp)
    {
        if (!hp) return false;

        var prof = stat ? stat.necromaner : null;
        if (prof == null) return false;

        var ctrl = hp.GetComponent<EnemyCtrl>();
        if (!ctrl || !ctrl.def) return false;

        if (ctrl.Faction != Faction.Corpse) return false;

        if (reserved.Contains(hp) || reanim.Contains(hp)) return false;

        bool boss = ctrl.def.boss != null;
        if (boss && !prof.bossOK) return false;

        float v = Random.value;

        if (v >= prof.reviveChance) return false;

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
        StartCoroutine(Reanimate(hp));
    }

    IEnumerator Reanimate(EnemyHp hp)
    {
        var ctrl = hp.GetComponent<EnemyCtrl>();
        if (!ctrl) { reanim.Remove(hp); yield break; }

        var prof = stat ? stat.necromaner : null;
        if (prof == null) { reanim.Remove(hp); yield break; }

        yield return new WaitForSeconds(prof.reviveDelay);

        if (!hp.gameObject.activeInHierarchy) { reanim.Remove(hp); yield break; }

        ctrl.Animation?.PlayResurrection();
        ctrl.SetFaction(Faction.Ally);

        int baseHp = 1;
        if (ctrl.def && ctrl.def.stats) baseHp = ctrl.def.stats.maxHp;

        int newHp = Mathf.Max(1, Mathf.RoundToInt(baseHp * prof.hpMul));
        hp.Revive(newHp, true);

        ctrl.SetAttackMul(prof.attackMul);

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

        var ctrl = hp.GetComponent<EnemyCtrl>();
        if (ctrl) undead.Remove(ctrl);

        hp.OnDied -= OnDied;
    }
}
