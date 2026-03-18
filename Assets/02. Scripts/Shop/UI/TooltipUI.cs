using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] CanvasGroup group;
    [SerializeField] RectTransform root;
    [SerializeField] TMP_Text descText;

    void Awake()
    {
        Hide();
    }

    void Update()
    {
        if (group.alpha > 0f)
            root.position = Input.mousePosition + Vector3.up * 100f;
    }

    public void Show(ItemDefSO item)
    {
        if (item == null) { Hide(); return; }

        descText.text = item.description;

        group.alpha = 1f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    public void Hide()
    {
        group.alpha = 0f;
    }

    public void Message(string msg)
    {
        descText.text = msg;
    }
}
