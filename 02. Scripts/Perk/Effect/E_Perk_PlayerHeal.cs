using UnityEngine;
using System.Collections.Generic;

using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Mods;
using Necrogue.Player.Runtime;



[CreateAssetMenu(menuName = "Perk/Effect/Player/Heal")]
public class PerkEffect_PlayerHeal : PerkEffect
{
    [SerializeField] int healValue = 30;

    public override void OnAcquire(Player player, int stack)
    {
        player.Hp.Heal(healValue);
    }
}