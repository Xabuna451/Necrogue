using UnityEngine;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUI;

    [SerializeField] private Slider expSlider;

    void Update()
    {
        // 플레이어 경험치에 따라 슬라이더 값 Lerp로 갱신 

        expSlider.maxValue = playerUI.Player.Exp.MaxExp;
        expSlider.value = Mathf.Lerp(expSlider.value, playerUI.Player.Exp.Exp, Time.deltaTime * 2f);
    }
}