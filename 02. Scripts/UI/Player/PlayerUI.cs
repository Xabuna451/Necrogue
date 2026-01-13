using UnityEngine;

using Necrogue.Player.Runtime;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Player player;         // 인스펙터에서 플레이어 오브젝트 드래그
    [SerializeField] private PlayerHpBar hpBar;
    [SerializeField] private PlayerExpBar expBar;
    [SerializeField] private DamageFlashUI damageFlashUI;

    public Player Player => player;
    public PlayerHpBar HpBar => hpBar;
    public PlayerExpBar ExpBar => expBar;
    public DamageFlashUI DamageFlashUI => damageFlashUI;

    private void Awake()
    {
        // 필요하면 여기서 hpBar, expBar에 player 전달
        // 또는 각 바 스크립트에서 PlayerUI 참조해서 가져가게 해도 OK
    }
}