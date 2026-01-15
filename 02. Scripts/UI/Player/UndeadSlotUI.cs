using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Necrogue.UI.Player
{
    public class UndeadSlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text countText;

        public void Bind(Sprite sprite, int count)
        {
            if (icon) icon.sprite = sprite;
            if (countText) countText.text = count.ToString();
        }
    }
}
