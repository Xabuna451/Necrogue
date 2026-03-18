using UnityEngine;
using System.Collections.Generic;

using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Mods;
using Necrogue.Player.Runtime;



[CreateAssetMenu(menuName = "Perk/Effect/Player/FullHeal")]
public class PerkEffect_PlayerFullHeal : PerkEffect
{
    public override void OnAcquire(Player player, int stack)
    {
        player.Hp.FullHeal();
    }
}