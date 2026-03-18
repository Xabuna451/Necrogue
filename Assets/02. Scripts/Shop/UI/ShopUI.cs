using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] ShopSO shop;                 // 교체용
    [SerializeField] Transform slotRoot;          // 슬롯들 부모(그리드)
    [SerializeField] ShopItemSlotUI slotPrefab;   // 슬롯 프리팹(선택)

    ShopItemSlotUI[] slots;

    void Awake()
    {
        // 씬에 배치한 슬롯 찾기
        slots = slotRoot.GetComponentsInChildren<ShopItemSlotUI>(true);

        BindShop(shop);
    }

    public void BindShop(ShopSO shopSO)
    {
        shop = shopSO;
        var items = (shop != null) ? shop.items : null;

        // 1) 슬롯 초기화
        for (int i = 0; i < slots.Length; i++)
            slots[i].gameObject.SetActive(false);

        if (items == null) return;

        // 2) 아이템 -> 슬롯 바인딩 (짧은 쪽 기준)
        int count = Mathf.Min(items.Length, slots.Length);

        for (int i = 0; i < count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].Bind(items[i]);
        }
    }

#if UNITY_EDITOR
    // 에디터에서 shop 바꾸면 바로 반영되고 싶으면
    void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (shop != null) BindShop(shop);
    }
#endif
}
