using UnityEngine;

using Necrogue.Player.Runtime;
using Necrogue.Core.Domain.Stats;
using System;

public class PlayerHp : MonoBehaviour, IDamageable
{
    private Player player;

    [Header("HP 설정")]
    [SerializeField] private int maxHp = 200;
    [SerializeField] private float invincibleTime = 0.5f;

    public int MaxHp => maxHp;

    public int CurrentHp { get; private set; }
    public bool IsInvincible { get; private set; }

    float invincibleEndTime;

    public bool IsDead => CurrentHp <= 0; // CurrentHp가 0 이하이면 true, 아니면 false를 반환

    public void Init(Player player)
    {
        this.player = player;
    }

    public void ApplyStats(PlayerRuntimeStats runtimeStats)
    {
        maxHp = runtimeStats.maxHp;
        CurrentHp = Mathf.Min(CurrentHp, maxHp);
    }

    public void ResetFull()
    {
        CurrentHp = maxHp;
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
        player.UI.DamageFlashUI.Play();
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




    // ==============외부 API / 디버그================
    public void Heal(int value)
    {
        CurrentHp = Mathf.Min(CurrentHp + value, maxHp);
    }
    public void FullHeal()
    {
        CurrentHp = maxHp;
    }
    public void SetHpDirect(int value)
    {
        CurrentHp = Mathf.Clamp(value, 0, maxHp);
    }
    // ============================================
}
