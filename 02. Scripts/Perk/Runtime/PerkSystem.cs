using UnityEngine;
using System;
using System.Collections.Generic;

using Necrogue.Core.Domain.Mods;
using Necrogue.Core.Domain.Compose;
using Necrogue.Core.Domain.Necro;
using Necrogue.Core.Domain.Stats;

using Necrogue.Game.Systems;
using Necrogue.Common.Data;

using Necrogue.Perk.UI;
using Necrogue.Perk.Data;

using Necrogue.Player.Runtime;

namespace Necrogue.Perk.Runtime
{
    public class PerkSystem : MonoBehaviour
    {
        [SerializeField] private int basePickCount = 3;
        public int PickCountBonus { get; private set; } = 0;

        public int PickCount => Mathf.Max(1, basePickCount + PickCountBonus);
        public void AddPickCountBonus(int bonus)
        {
            PickCountBonus = Mathf.Max(0, PickCountBonus + bonus);
            Debug.Log($"[PerkSystem] PickCountBonus updated: {PickCountBonus}");
        }
        // ==================================================
        // [0] Config / UI
        // ==================================================
        [Header("Perk Pool")]
        [SerializeField] private List<PerkDef> allPerks = new();

        [Header("Rarity Table")]
        [SerializeField] private PerkRarityTableSO rarityTable;


        [Header("UI")]
        [SerializeField] private PerkSelectUI perkSelectUI;

        // ==================================================
        // [1] State
        // ==================================================
        private readonly Dictionary<string, PerkInstance> ownedPerks = new();
        private readonly List<string> acquiredOrder = new();

        private readonly List<StatMod> statMods = new();
        private readonly List<NecroMod> necroMods = new();

        private Necrogue.Player.Runtime.Player player;

        // UI/ESC창 갱신용
        public event Action OnPerksChanged;

        // ==================================================
        // [2] Init
        // ==================================================
        public void Init(Necrogue.Player.Runtime.Player p) => player = p;

        // ==================================================
        // [UI] Owned perks (획득 순서대로)
        // ==================================================
        public IEnumerable<(PerkDef def, int stack)> EnumerateOwnedPerksByAcquireOrder()
        {
            for (int i = 0; i < acquiredOrder.Count; i++)
            {
                string id = acquiredOrder[i];
                if (!ownedPerks.TryGetValue(id, out var inst)) continue;
                if (inst == null || inst.def == null) continue;

                yield return (inst.def, inst.stack);
            }
        }

        // ==================================================
        // [3] Level Up -> UI Open
        // ==================================================
        public void OnLevelUp()
        {
            var picks = RollPerks(PickCount);
            perkSelectUI.Open(picks, this);

            // Time.timeScale 여기서 만지지 말 것
            // GameManager의 RuntimeState로 올려서 "멈춘 이유"를 명확히
            if (GameManager.Instance != null)
                GameManager.Instance.SetRuntimeState(RuntimeState.LevelUp);
        }

        // ==================================================
        // [4] Acquire
        // ==================================================
        public void AcquirePerk(PerkDef perk)
        {
            if (perk == null) return;

            bool firstAcquire = false;

            if (!ownedPerks.TryGetValue(perk.perkId, out var inst))
            {
                inst = new PerkInstance(perk);
                ownedPerks.Add(perk.perkId, inst);
                firstAcquire = true;
            }
            else
            {
                inst.stack = Mathf.Min(inst.stack + 1, perk.maxStack);
            }

            // 최초 획득일 때만 히스토리에 추가
            if (firstAcquire)
                acquiredOrder.Add(perk.perkId);

            // 먼저 계산 후 즉발효과 발동
            RecalculateAll();

            // null 안전 + perk.effects null일 수도 있으니 체크
            if (perk.effects != null)
            {
                foreach (var eff in perk.effects)
                    eff?.OnAcquire(player, inst.stack);
            }

            // UI 갱신 이벤트
            OnPerksChanged?.Invoke();

            // Time.timeScale 여기서 만지지 말 것
            // LevelUp 끝났으니 Playing 복귀(GameOver 같은 상태면 건드리면 안됨)
            if (GameManager.Instance != null && GameManager.Instance.RuntimeState == RuntimeState.LevelUp)
                GameManager.Instance.SetRuntimeState(RuntimeState.Playing);
        }

        // ==================================================
        // [5] Recalculate
        // ==================================================
        public void RecalculateAll()
        {
            if (player == null || player.Stats == null) return;

            player.RuntimeStats.SetFromBase(player.Stats);
            player.NecroRuntime.Reset();

            statMods.Clear();
            necroMods.Clear();

            foreach (var kv in ownedPerks)
            {
                var inst = kv.Value;
                if (inst == null || inst.def == null || inst.def.effects == null) continue;

                foreach (var eff in inst.def.effects)
                {
                    if (eff == null) continue;
                    eff.CollectStat(statMods, inst.stack);
                    eff.CollectNecro(necroMods, inst.stack);
                }
            }

            StatComposer.Apply(player.RuntimeStats, statMods);
            NecroComposer.Apply(player.NecroRuntime, necroMods);

            player.ApplyRuntimeStats();
        }

        // ==================================================
        // [6] Roll
        // ==================================================
        private PerkDef[] RollPerks(int count)
        {
            count = Mathf.Max(1, count);

            // maxStack 제외 후보
            var basePool = new List<PerkDef>();
            foreach (var p in allPerks)
            {
                if (p == null) continue;
                if (IsMaxStack(p)) continue;
                basePool.Add(p);
            }

            int n = Mathf.Min(count, basePool.Count);
            var result = new PerkDef[n];

            // 같은 레벨업 내 중복 방지
            var pickedIds = new HashSet<string>();

            for (int i = 0; i < n; i++)
            {
                var pick = PickOneByRarityThenWeight(basePool, pickedIds);
                if (pick == null) break;

                result[i] = pick;
                pickedIds.Add(pick.perkId);
                basePool.Remove(pick); // 같은 레벨업에서 중복 방지
            }

            return result;
        }

        private PerkDef PickOneByRarityThenWeight(List<PerkDef> pool, HashSet<string> pickedIds)
        {
            if (pool == null || pool.Count == 0) return null;

            // 1) rarity 먼저 뽑기
            var rarity = RollRarity();
            Debug.Log($"[PerkSystem] Rolled Rarity: {rarity}");

            // 2) 그 rarity 후보 만들기
            var candidates = GetCandidates(pool, pickedIds, rarity);

            // 3) 비었으면 다운그레이드 (Fable->...->Common)
            if (candidates.Count == 0)
            {
                rarity = DowngradeToAvailable(pool, pickedIds, rarity);
                candidates = GetCandidates(pool, pickedIds, rarity);
            }

            // 4) 그래도 없으면 남은 풀 아무거나
            if (candidates.Count == 0)
                candidates = GetCandidatesAny(pool, pickedIds);

            if (candidates.Count == 0) return null;

            int idx = PickWeightedIndex(candidates); // 내부 weight 사용
            return candidates[idx];
        }

        private List<PerkDef> GetCandidates(List<PerkDef> pool, HashSet<string> picked, PerkRarity rarity)
        {
            var list = new List<PerkDef>();
            for (int i = 0; i < pool.Count; i++)
            {
                var p = pool[i];
                if (p == null) continue;
                if (picked != null && picked.Contains(p.perkId)) continue;
                if (p.rarity != rarity) continue;
                list.Add(p);
            }
            return list;
        }

        private List<PerkDef> GetCandidatesAny(List<PerkDef> pool, HashSet<string> picked)
        {
            var list = new List<PerkDef>();
            for (int i = 0; i < pool.Count; i++)
            {
                var p = pool[i];
                if (p == null) continue;
                if (picked != null && picked.Contains(p.perkId)) continue;
                list.Add(p);
            }
            return list;
        }

        private PerkRarity DowngradeToAvailable(List<PerkDef> pool, HashSet<string> picked, PerkRarity start)
        {
            for (int r = (int)start; r >= (int)PerkRarity.Common; r--)
            {
                var rr = (PerkRarity)r;
                if (GetCandidates(pool, picked, rr).Count > 0) return rr;
            }
            return start;
        }


        private PerkRarity RollRarity()
        {
            // 테이블 없으면 안전 기본값
            if (rarityTable == null || rarityTable.entries == null || rarityTable.entries.Length == 0)
                return PerkRarity.Common;

            float total = 0f;
            for (int i = 0; i < rarityTable.entries.Length; i++)
                total += Mathf.Max(0f, rarityTable.entries[i].weight);

            if (total <= 0.0001f) return PerkRarity.Common;

            float r = UnityEngine.Random.Range(0f, total);
            float acc = 0f;

            for (int i = 0; i < rarityTable.entries.Length; i++)
            {
                acc += Mathf.Max(0f, rarityTable.entries[i].weight);
                if (r <= acc) return rarityTable.entries[i].rarity;
            }

            return rarityTable.entries[rarityTable.entries.Length - 1].rarity;
        }

        private int PickWeightedIndex(List<PerkDef> pool)
        {
            float total = 0f;
            for (int i = 0; i < pool.Count; i++)
                total += Mathf.Max(0f, pool[i].weight);

            if (total <= 0.0001f)
                return UnityEngine.Random.Range(0, pool.Count);

            float r = UnityEngine.Random.Range(0f, total);
            float acc = 0f;

            for (int i = 0; i < pool.Count; i++)
            {
                acc += Mathf.Max(0f, pool[i].weight);
                if (r <= acc) return i;
            }

            return pool.Count - 1;
        }

        public int GetStack(string perkId)
        {
            if (string.IsNullOrEmpty(perkId)) return 0;
            return ownedPerks.TryGetValue(perkId, out var inst) ? inst.stack : 0;
        }

        public bool IsMaxStack(PerkDef perk)
        {
            if (perk == null) return true;
            int stack = GetStack(perk.perkId);
            return stack >= Mathf.Max(1, perk.maxStack);
        }
    }
}
