using UnityEngine;
using System.Collections.Generic;
using Necrogue.Core.Domain.Mods;
using Necrogue.Core.Domain.Stats;

namespace Necrogue.Core.Domain.Compose
{
    public static class StatComposer
    {
        public static void Apply(PlayerRuntimeStats s, List<StatMod> mods)
        {
            float addHp = 0, mulHp = 0; bool hasHpOv = false; float hpOv = 0;
            float addAtk = 0, mulAtk = 0; bool hasAtkOv = false; float atkOv = 0;
            float addSpd = 0, mulSpd = 0; bool hasSpdOv = false; float spdOv = 0;

            foreach (var m in mods)
            {
                switch (m.stat)
                {
                    case StatId.MaxHp:
                        if (m.type == ModType.Add) addHp += m.value;
                        else if (m.type == ModType.Mul) mulHp += m.value;
                        else { hasHpOv = true; hpOv = m.value; }
                        break;

                    case StatId.Attack:
                        if (m.type == ModType.Add) addAtk += m.value;
                        else if (m.type == ModType.Mul) mulAtk += m.value;
                        else { hasAtkOv = true; atkOv = m.value; }
                        break;

                    case StatId.Speed:
                        if (m.type == ModType.Add) addSpd += m.value;
                        else if (m.type == ModType.Mul) mulSpd += m.value;
                        else { hasSpdOv = true; spdOv = m.value; }
                        break;
                }
            }

            float hp = (s.maxHp + addHp) * (1f + mulHp);
            float atk = (s.attack + addAtk) * (1f + mulAtk);
            float spd = (s.speed + addSpd) * (1f + mulSpd);

            if (hasHpOv) hp = hpOv;
            if (hasAtkOv) atk = atkOv;
            if (hasSpdOv) spd = spdOv;

            s.maxHp = Mathf.Max(1, Mathf.RoundToInt(hp));
            s.attack = Mathf.Max(0, Mathf.RoundToInt(atk));
            s.speed = Mathf.Max(0.01f, spd);
        }
    }
}
