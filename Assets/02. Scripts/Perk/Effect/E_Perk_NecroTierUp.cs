using UnityEngine;
using System.Collections.Generic;
using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Mods;
using Necrogue.Core.Domain.Necro;

[CreateAssetMenu(menuName = "Perk/Effect/Necro/TierUp")]
public class PerkEffect_NecroTierUp : PerkEffect
{
    [Header("Target")]
    [SerializeField] private NecroParam param = NecroParam.AllyDamage;

    [Header("Type")]
    [SerializeField] private ModType type = ModType.Add;

    [Header("Rule")]
    [Tooltip("몇 스택마다 발동 (예: 5)")]
    [SerializeField] private int step = 5;

    [Tooltip("발동 1회당 보너스. Add면 플랫, Mul이면 퍼센트(0.10=+10%)")]
    [SerializeField] private float bonusPerStep = 1f;

    public override void CollectNecro(List<NecroMod> mods, int stack)
    {
        int s = Mathf.Max(0, stack);
        if (step <= 0 || bonusPerStep == 0f) return;

        int k = s / step;          // 예) 5->1, 10->2
        if (k <= 0) return;

        float value = bonusPerStep * k;
        mods.Add(new NecroMod(param, type, value));
    }
}
