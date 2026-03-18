using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ShopItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TooltipUI tooltip; // 상점 UI에 있는 Tooltip 참조
    [SerializeField] ItemDefSO item;       // 이 슬롯이 들고 있는 아이템 SO

    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameAndpriceText;

    void Awake()
    {
        // 가격을 동적으로 가져오도록 변경
    }

    public void Bind(ItemDefSO itemSO)
    {
        item = itemSO;

        if (item == null)
        {
            iconImage.sprite = null;
            nameAndpriceText.text = "";
            return;
        }

        iconImage.sprite = item.icon;
        UpdatePrice();
    }

    /// <summary>현재 아이템의 구매 개수에 따라 가격을 업데이트</summary>
    private void UpdatePrice()
    {
        if (item == null) return;

        int haveCount = SaveManager.Instance.GetHaveItemCount(item.itemId);
        
        // price 배열의 범위 내에서 가격 결정 (범위 초과 시 마지막 가격)
        int priceIndex = Mathf.Min(haveCount, item.price.Length - 1);
        int currentPrice = item.price[priceIndex];
        
        nameAndpriceText.text = $"{item.displayName}\n{currentPrice} Gold";
        
        Debug.Log($"Item {item.itemId} - Have: {haveCount}, PriceIndex: {priceIndex}, Price: {currentPrice}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip == null) return;
        tooltip.Show(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip == null) return;
        tooltip.Hide();
    }

    public void BuyItem()
    {
        if (!item) return;

        // 현재 가격 가져오기 (구매 전)
        int haveCount = SaveManager.Instance.GetHaveItemCount(item.itemId);
        int priceIndex = Mathf.Min(haveCount, item.price.Length - 1);
        int price = item.price[priceIndex];

        if (SaveManager.Instance.SpendGold(price) == true)
        {
            item.itemEffect.Effect();
            SaveManager.Instance.AddHaveItem(item.itemId, 1);

            // 구매 후 가격 업데이트
            UpdatePrice();
        }
        else if(SaveManager.Instance.Data.metaGold < price)
        {
            tooltip.Message("골드가 부족합니다!");
        }
    }
}
