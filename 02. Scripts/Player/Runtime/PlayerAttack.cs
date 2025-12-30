using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 스탯")]
    [SerializeField] private int attackPower = 10;

    private Player player;

    public int AttackPower => attackPower;

    public void Init(Player player)
    {
        this.player = player;
    }

    public void InitFromBase(PlayerStatAsset stats)
    {
        if (stats)
        {
            attackPower = stats.baseAttack;
        }
    }

    // ================== 공격 실행 ==================
    // 지금은 "공격력만 가진다"
    // 실제 공격은 Bullet / Hitbox / Skill에서 사용
    // ==============================================


    // ============== 외부 API (디버그/치트) ==============
    public void SetAttackDirect(int value)
    {
        attackPower = Mathf.Max(0, value);
        Debug.Log($"[DEBUG] Player Attack set to {attackPower}");
    }
    // ====================================================
}
