using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Concat, Select 등 LINQ 사용

using Necrogue.Enemy.Runtime;
using Necrogue.Enemy.Data.States;

namespace Necrogue.Player.Runtime
{
    public partial class NecromancerController : MonoBehaviour
    {
        [SerializeField] private PlayerStatAsset stat;

        [SerializeField] private NecroPerkState perk;

        [Header("Elite / Boss Undead 허용 여부")]
        [SerializeField] private bool isEliteUndead = false;
        [SerializeField] private bool isBossUndead = false;

        private readonly List<EnemyContext> undead = new();
        private readonly HashSet<EnemyHp> reserved = new();
        private readonly HashSet<EnemyHp> reanim = new();

        private int BaseMax => stat?.necromaner?.maxCount ?? 0;
        private int Slots => undead.Count + reserved.Count + reanim.Count;
        private int Max => BaseMax + (perk?.AllyCapBonus ?? 0);
        public bool HasSlot => Slots < Max;

        private void Awake()
        {
            if (!perk) perk = GetComponent<NecroPerkState>();
            if (perk) perk.OnChanged += RefreshUndeadStats;
        }

        private void OnDestroy()
        {
            if (perk) perk.OnChanged -= RefreshUndeadStats;

            // 모든 추적 중인 HP의 OnDied 이벤트 정리 (메모리 누수 방지)
            var allTrackedHp = reserved.Concat(reanim)
                                        .Concat(undead.Select(c => c?.Hp))
                                        .Where(hp => hp != null);

            foreach (var hp in allTrackedHp)
            {
                hp.OnDied -= OnDied;
            }
        }

        // =============================================================
        // Perk 변경 시 모든 언데드 스탯 갱신
        // =============================================================
        private void RefreshUndeadStats()
        {
            for (int i = undead.Count - 1; i >= 0; i--)
            {
                var ctrl = undead[i];
                if (!ctrl)
                {
                    undead.RemoveAt(i);
                    continue;
                }

                if (ctrl.Faction != Faction.Ally) continue;

                var hp = ctrl.Hp;
                if (hp == null || hp.Dead) continue;

                ApplyUndeadStats(ctrl);
            }

            NotifyUndeadChanged();
        }

        // HP + Attack 한 번에 적용하는 통합 헬퍼
        private void ApplyUndeadStats(EnemyContext ctrl)
        {
            if (!ctrl || !ctrl.Hp || !ctrl.def) return;

            var prof = stat?.necromaner;
            if (prof == null || perk == null) return;

            // HP 적용
            int baseMax = Mathf.Max(1, ctrl.def.stats?.maxHp ?? 1);
            int newMax = NecroUndeadStatFormula.ComputeMaxHp(
                baseMax,
                prof.hpMul,
                perk.AllyHpAdd,
                perk.AllyHpMul
            );
            ctrl.Hp.SetMaxHp(newMax);

            // Attack 적용
            float baseAtk = ctrl.def.attack?.attackDamage ?? 1f;
            float mul = NecroUndeadStatFormula.ComputeAttackMul(
                baseAtk,
                prof.attackMul,
                perk.AllyDamageAdd,
                perk.AllyDamageMul
            );
            ctrl.SetAttackMul(mul);
        }

        // =============================================================
        // 부활 예약 시도
        // =============================================================
        public bool TryReserve(EnemyHp hp)
        {
            if (!IsValidForReserve(hp)) return false;

            reserved.Add(hp);
            NotifyUndeadChanged();
            return true;
        }

        private bool IsValidForReserve(EnemyHp hp)
        {
            if (!hp) return false;

            var prof = stat?.necromaner;
            if (prof == null) return false;

            var ctrl = hp.GetComponent<EnemyContext>();
            if (!ctrl || ctrl.Faction != Faction.Corpse) return false;

            if (reserved.Contains(hp) || reanim.Contains(hp)) return false;

            // 보스/엘리트 조건
            if (ctrl.def?.boss != null && !prof.bossOK && !isBossUndead) return false;
            if (ctrl.IsElite && !isEliteUndead) return false;

            // 레벨 조건
            var player = GetComponentInParent<Player>();
            if (player?.Necro?.Level < ctrl.def?.stats?.underLevel) return false;

            // 확률
            if (Random.value >= prof.reviveTime) return false;

            // 슬롯 여유
            if (!HasSlot)
            {
                Debug.Log($"[Necro] Slot full → {Slots}/{Max}");
                return false;
            }

            return true;
        }

        // =============================================================
        // 부활 준비 완료 (예약 -> 부활 대기열 이동)
        // =============================================================
        public void OnCorpseReady(EnemyHp hp)
        {
            if (!hp || !reserved.Contains(hp)) return;
            if (reanim.Contains(hp)) return;

            reserved.Remove(hp);
            reanim.Add(hp);
            NotifyUndeadChanged();

            var ctrl = hp.GetComponent<EnemyContext>();
            if (ctrl) ctrl.StateMachine.SwitchState(EnemyStateType.Corpse);

            StartCoroutine(Reanimate(hp));
        }

        // =============================================================
        // 실제 부활 코루틴
        // =============================================================
        private IEnumerator Reanimate(EnemyHp hp)
        {
            var ctrl = hp?.GetComponent<EnemyContext>();
            if (!ctrl) { reanim.Remove(hp); yield break; }

            var prof = stat?.necromaner;
            if (prof == null) { reanim.Remove(hp); yield break; }

            yield return new WaitForSeconds(prof.reviveDelay);

            if (!hp.gameObject.activeInHierarchy) { reanim.Remove(hp); yield break; }

            ctrl.StateMachine.SwitchState(EnemyStateType.Revive);

            // 부활 시 풀 힐 + 스탯 적용
            ApplyUndeadStats(ctrl);
            ctrl.Hp?.Revive(ctrl.Hp.MaxHp, fullHeal: true);  // EnemyHp의 Revive 호출 (fullHeal)

            if (!undead.Contains(ctrl))
            {
                undead.Add(ctrl);
                hp.OnDied -= OnDied;
                hp.OnDied += OnDied;
            }

            reanim.Remove(hp);
            NotifyUndeadChanged();
        }

        private void OnDied(EnemyHp hp, Faction diedAs)
        {
            if (diedAs != Faction.Ally) return;

            var ctrl = hp?.GetComponent<EnemyContext>();
            if (ctrl) undead.Remove(ctrl);

            hp.OnDied -= OnDied;
            NotifyUndeadChanged();
        }

        // =============================================================
        // 외부 API
        // =============================================================
        public void EliteUndead(bool on) => isEliteUndead = on;
        public void BossUndead(bool on) => isBossUndead = on;
    }
}