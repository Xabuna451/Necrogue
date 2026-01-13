using UnityEngine;
using System.Collections.Generic;

using Necrogue.Core.Domain.Mods;
using Necrogue.Core.Domain.Compose;
using Necrogue.Core.Domain.Necro;
using Necrogue.Core.Domain.Stats;

using Necrogue.Perk.UI;

// PerkDef/PerkInstance/PerkEffect가 여기 있다면 필요
using Necrogue.Perk.Data;

namespace Necrogue.Perk.Runtime
{
    public class PerkSystem : MonoBehaviour
    {
        [SerializeField] private int basePickCount = 3;
        public int PickCountBonus { get; private set; } = 0;   // 상점/메타에서 올리는 값

        public int PickCount => Mathf.Max(1, basePickCount + PickCountBonus);
        public void AddPickCountBonus(int bonus)
        {
            PickCountBonus = Mathf.Max(0, PickCountBonus + bonus);
        }
        // ==================================================
        // [0] Config / UI
        // ==================================================
        [Header("Perk Pool")]
        [SerializeField] private List<PerkDef> allPerks = new();

        [Header("UI")]
        [SerializeField] private PerkSelectUI perkSelectUI;

        // ==================================================
        // [1] State
        // ==================================================
        private readonly Dictionary<string, PerkInstance> ownedPerks = new();

        private readonly List<StatMod> statMods = new();
        private readonly List<NecroMod> necroMods = new();

        // 플레이어는 명시(풀네임)
        private Necrogue.Player.Runtime.Player player;

        // ==================================================
        // [2] Init
        // ==================================================
        public void Init(Necrogue.Player.Runtime.Player p) => player = p;

        // ==================================================
        // [3] Level Up -> UI Open
        // ==================================================
        public void OnLevelUp()
        {
            var picks = RollPerks(PickCount);
            perkSelectUI.Open(picks, this);
            Time.timeScale = 0f;
        }

        // ==================================================
        // [4] Acquire
        // ==================================================
        public void AcquirePerk(PerkDef perk)
        {
            if (perk == null) return;

            if (!ownedPerks.TryGetValue(perk.perkId, out var inst))
            {
                inst = new PerkInstance(perk);
                ownedPerks.Add(perk.perkId, inst);
            }
            else
            {
                inst.stack = Mathf.Min(inst.stack + 1, perk.maxStack);

                // PerkInstance가 struct이면 아래가 필요
                // ownedPerks[perk.perkId] = inst;
            }

            // 먼저 계산 후 즉발효과 발동. 먼저 계산 안하면 MaxAdd30 됐을 때 체력 먼저 차고 그 다음 최대체력 늘어남 
            RecalculateAll();

            foreach (var eff in perk.effects)
            {
                eff?.OnAcquire(player, inst.stack);
            }

            Time.timeScale = 1f;
        }

        // ==================================================
        // [5] Recalculate (Perk List -> Mods -> Compose -> Apply)
        // ==================================================
        public void RecalculateAll()
        {
            if (player == null || player.Stats == null) return;

            // 1) Base -> Runtime 복사
            player.RuntimeStats.SetFromBase(player.Stats);
            player.NecroRuntime.Reset();

            // 2) Mods 수집
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

            // 3) 합성 적용
            StatComposer.Apply(player.RuntimeStats, statMods);
            NecroComposer.Apply(player.NecroRuntime, necroMods);

            // 4) Player에 반영
            player.ApplyRuntimeStats();
        }

        // ==================================================
        // [6] Roll
        // ==================================================
        private PerkDef[] RollPerks(int count)
        {
            count = Mathf.Max(1, count);

            // 후보 풀: maxStack 도달 제외
            var pool = new List<PerkDef>();
            foreach (var p in allPerks)
            {
                if (p == null) continue;
                if (IsMaxStack(p)) continue;
                pool.Add(p);
            }

            // 실제로 뽑을 수 있는 만큼만
            int n = Mathf.Min(count, pool.Count);
            var result = new PerkDef[n];

            for (int i = 0; i < n; i++)
            {
                int idx = PickWeightedIndex(pool);
                result[i] = pool[idx];
                pool.RemoveAt(idx);
            }

            return result;
        }

        private int PickWeightedIndex(List<PerkDef> pool)
        {
            float total = 0f;
            for (int i = 0; i < pool.Count; i++)
                total += Mathf.Max(0f, pool[i].weight);

            // 전부 0이면 균등
            if (total <= 0.0001f)
                return Random.Range(0, pool.Count);

            float r = Random.Range(0f, total);
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
