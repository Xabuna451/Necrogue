using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item")]
public class ItemDefSO : ScriptableObject
{
    public string displayName;
    [Header("아이템 고유 ID")]
    public int itemId;
    [TextArea(3, 10)] public string description;
    public Sprite icon;

    [Header("가격 단계별 설정")]
    public int[] price;
    public ItemEffect itemEffect;
}