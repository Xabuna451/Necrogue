using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private Slider hpSlider;

    void Update()
    {
        hpSlider.maxValue = playerUI.Player.Stats.baseMaxHp;
        hpSlider.value = Mathf.Lerp(hpSlider.value, playerUI.Player.Hp.CurrentHp, Time.deltaTime * 5f);
    }
}