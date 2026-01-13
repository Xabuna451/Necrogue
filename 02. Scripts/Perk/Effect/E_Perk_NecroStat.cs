using UnityEngine;
using System.Collections.Generic;
using Necrogue.Core.Domain.Necro;
using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Mods;

[CreateAssetMenu(menuName = "Perk/Effect/Necro/Stat")]
public class PerkEffect_NecroStat : PerkEffect
{
    [Header("Target")]
    [SerializeField] private NecroParam param = NecroParam.AllyDamage;

    [Header("Type")]
    [SerializeField] private ModType type = ModType.Add;

    [Header("Add (합산)")]
    [Tooltip("스택당 더해질 값(플랫).")]
    [SerializeField] private float addPerStack = 0f;

    [Header("Mul (퍼센트 누적)")]
    [Tooltip("스택당 더해질 퍼센트. 예) 0.03 = +3%/스택")]
    [SerializeField] private float mulPerStack = 0f;

    public override void CollectNecro(List<NecroMod> mods, int stack)
    {
        int s = Mathf.Max(0, stack);

        float value =
            (type == ModType.Add) ? (addPerStack * s) :
            (type == ModType.Mul) ? (mulPerStack * s) :
            0f;

        mods.Add(new NecroMod(param, type, value));
    }
}
