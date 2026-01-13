using UnityEngine;
using System.Collections.Generic;
using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Mods;

[CreateAssetMenu(menuName = "Perk/Effect/Player/TierUp")]
public class PerkEffect_PlayerTierUp : PerkEffect
{
    [Header("Target")]
    [SerializeField] private StatId stat = StatId.Attack;
    [Header("Type")]
    [SerializeField] private ModType type = ModType.Add;

    [Tooltip("몇 스택마다 발동 (예: 5)")]
    [SerializeField] private int step = 5;

    [Tooltip("발동 1회당 보너스. Add면 플랫, Mul이면 퍼센트(0.10=+10%)")]
    [SerializeField] private float bonusPerStep = 1f;

    public override void CollectStat(List<StatMod> stats, int stack)
    {
        int s = Mathf.Max(0, stack);
        if (step <= 0 || bonusPerStep == 0f) return;

        int k = s / step;               // 5면 1, 10이면 2
        float value = bonusPerStep * k; // 누적 보너스

        stats.Add(new StatMod(stat, type, value));
    }
}
