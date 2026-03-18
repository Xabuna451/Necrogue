using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Necrogue.Perk.Data;

public class PerkIconSlotUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text stackText;

    public void Bind(PerkDef def, int stack)
    {
        if (icon) icon.sprite = def != null ? def.icon : null;

        // 아이작식: 스택이 2 이상일 때만 숫자 표시
        if (stackText)
        {
            bool show = stack >= 2;
            stackText.gameObject.SetActive(show);
            if (show) stackText.text = stack.ToString();
        }
    }
}
