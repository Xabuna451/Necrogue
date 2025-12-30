using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    private Player player;

    [Header("HP 설정")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private float invincibleTime = 0.5f;

    public int CurrentHp { get; private set; }
    public bool IsInvincible { get; private set; }

    float invincibleEndTime;

    public void Init(Player player)
    {
        this.player = player;
    }

    public void InitFromBase(PlayerStatAsset stats)
    {
        player.Stats = stats;

        if (stats)
        {
            maxHp = stats.baseMaxHp;
            CurrentHp = maxHp;
        }
    }

    void Start()
    {
        IsInvincible = false;
    }

    void Update()
    {
        // 무적 시간 종료 처리
        if (IsInvincible && Time.time >= invincibleEndTime)
        {
            IsInvincible = false;
        }
    }

    /// <summary>
    /// 외부에서 호출하는 데미지 처리 함수
    /// </summary>
    public void Damaged(int damage)
    {
        if (damage <= 0) return;
        if (IsInvincible) return;
        if (CurrentHp <= 0) return;

        CurrentHp -= damage;
        if (CurrentHp < 0)
            CurrentHp = 0;

        // 피격 후 무적
        IsInvincible = true;
        invincibleEndTime = Time.time + invincibleTime;

        OnDamaged(damage);

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    // ---- 내부 처리 ----

    void OnDamaged(int damage)
    {
        // 여기서:
        // - 피격 이펙트
        // - 화면 깜빡임
        // - 사운드
        // - 넉백 트리거
        // 등을 연결하면 됨
        Debug.Log($"Player damaged: -{damage}, HP={CurrentHp}");
    }

    void Die()
    {
        Debug.Log("Player Dead");

        // 여기서:
        // - 입력 비활성화
        // - 사망 애니메이션
        // - 게임 오버 트리거
    }




    // ==============외부 API 디버그================
    public void SetHpDirect(int value)
    {
        CurrentHp = Mathf.Clamp(value, 0, maxHp);
    }
    // ============================================
}
