using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private float smooth = 5f;

    void Update()
    {
        if (!playerUI || !playerUI.Player || !playerUI.Player.Hp || !hpSlider)
            return;

        var hp = playerUI.Player.Hp;

        hpSlider.maxValue = hp.MaxHp;

        // 값은 부드럽게
        float target = hp.CurrentHp;
        hpSlider.value = Mathf.Lerp(hpSlider.value, target, Time.deltaTime * smooth);
    }
}
