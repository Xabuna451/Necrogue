using UnityEngine;

using Necrogue.Perk.Data.Perk;
using Necrogue.Player.Runtime;

[CreateAssetMenu(menuName = "Perk/Effect/Necro/LevelUp")]
public class E_Perk_NecroLevelUp : PerkEffect
{
    public override void OnAcquire(Player player, int stack)
    {
        player.Necro.NecromancerLevelUp();
    }
}
