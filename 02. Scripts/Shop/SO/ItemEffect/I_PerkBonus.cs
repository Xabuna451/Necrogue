using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Effect/PerkBonus")]

public class I_PerkBonus : ItemEffect
{
    public override void Effect()
    {
        SaveManager.Instance.AddPerkBonus(1);
    }
}