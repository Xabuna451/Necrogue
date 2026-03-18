using UnityEngine;

[CreateAssetMenu(menuName = "Shop/ItemEffect")]

public abstract class ItemEffect : ScriptableObject
{
    public abstract void Effect();
}