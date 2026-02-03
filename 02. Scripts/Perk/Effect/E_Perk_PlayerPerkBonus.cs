using UnityEngine;

using Necrogue.Perk.Data.Perk;
using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Mods;
using Necrogue.Player.Runtime;

[CreateAssetMenu(menuName = "Perk/Effect/Player/PerkBonus")]
public class PerkEffect_PlayerPerkBonus : PerkEffect
{
    [SerializeField] int bonus = 1;
    public override void OnAcquire(Player player, int stack)
    {
        player.Perks.AddPickCountBonus(bonus);
    }
}