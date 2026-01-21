using UnityEngine;
using System.Collections.Generic;
using Necrogue.Core.Domain.Necro;
using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Mods;
using Necrogue.Player.Runtime;


[CreateAssetMenu(menuName = "Perk/Effect/Player/EliteUndead")]
public class E_Perk_PlayerEliteUndead : PerkEffect
{
    public override void OnAcquire(Player player, int stack)
    {
        var necro = player.GetComponentInChildren<NecromancerController>();
        if (!necro) return;

        necro.EliteUndead(true);
    }
}
