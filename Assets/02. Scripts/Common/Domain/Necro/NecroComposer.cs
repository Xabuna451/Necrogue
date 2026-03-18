using UnityEngine;
using System.Collections.Generic;
using Necrogue.Core.Domain.Mods;

namespace Necrogue.Core.Domain.Necro
{
    public static class NecroComposer
    {
        public static void Apply(NecroRuntimeParams p, List<NecroMod> mods)
        {
            p.Reset();

            foreach (var m in mods)
            {
                switch (m.param)
                {
                    case NecroParam.AllyDamage:
                        if (m.type == ModType.Add) p.allyDamageAdd += m.value;
                        else if (m.type == ModType.Mul) p.allyDamageMul += m.value;
                        else { p.hasAllyDamageOv = true; p.allyDamageOv = m.value; }
                        break;

                    case NecroParam.AllyHp:
                        if (m.type == ModType.Add) p.allyHpAdd += m.value;
                        else if (m.type == ModType.Mul) p.allyHpMul += m.value;
                        else { p.hasAllyHpOv = true; p.allyHpOv = m.value; }
                        break;

                    case NecroParam.AllyCap:
                        if (m.type == ModType.Add) p.allyCapAdd += Mathf.RoundToInt(m.value);
                        else if (m.type == ModType.Mul) p.allyCapMul += m.value;
                        else { p.hasAllyCapOv = true; p.allyCapOv = Mathf.RoundToInt(m.value); }
                        break;
                }
            }
        }
    }
}
