using UnityEngine;
using System.Collections.Generic;
using Necrogue.Core.Domain.Necro;
using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Mods;

/// <summary>
/// 얘도 기본적으로 플레이어 스탯 조작은 이 SO로 해결
/// </summary>
[CreateAssetMenu(menuName = "Perk/Effect/Player/Stat")]
public class PerkEffect_PlayerStat : PerkEffect
{
    [Header("Target")]
    [SerializeField] private StatId param = StatId.MaxHp;
    [Header("Type")]
    [SerializeField] private ModType type = ModType.Add;

    [Header("Add (합산)")]
    [Tooltip("스택당 더해질 값.")]
    [SerializeField] private float addPerStack = 0f;

    [Header("Mul (곱연산)")]
    [Tooltip("스택당 곱해질 배율. 예) 1.03 = 3%/스택")]
    [SerializeField] private float mulPerStack = 1f;

    public override void CollectStat(List<StatMod> stats, int stack)
    {
        int s = Mathf.Max(0, stack);

        float value =
            (type == ModType.Add) ? addPerStack * s :
            (type == ModType.Mul) ? mulPerStack * s :
            0f;

        stats.Add(new StatMod(param, type, value));
    }
}
