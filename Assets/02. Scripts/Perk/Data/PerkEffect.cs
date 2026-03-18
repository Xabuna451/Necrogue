using UnityEngine;
using System.Collections.Generic;
using Necrogue.Core.Domain.Mods;
using Necrogue.Core.Domain.Necro;
using Necrogue.Player.Runtime;


namespace Necrogue.Perk.Data.Perk
{
    public abstract class PerkEffect : ScriptableObject
    {
        public virtual void CollectStat(List<StatMod> stats, int stack) { }
        public virtual void CollectNecro(List<NecroMod> necro, int stack) { }

        // 즉발 효과
        public virtual void OnAcquire(Necrogue.Player.Runtime.Player player, int stack) { }
    }
}