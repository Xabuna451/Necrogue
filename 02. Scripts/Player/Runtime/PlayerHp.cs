using UnityEngine;
using System;

using Necrogue.Core.Domain.Stats;
using Necrogue.Game.Systems;
using Necrogue.Common.Interfaces;
using Necrogue.Core.Domain.Necro;

namespace Necrogue.Player.Runtime
{
    public class PlayerHp : MonoBehaviour, IDamageable, IStatAppliable
    {
        private Player player;

        [Header("HP 설정")]
        [SerializeField] private int baseMaxHp = 200;           // 에디터에서 기본값 설정용 (스탯 미적용 시)
        [SerializeField] private float invincibleTime = 0.5f;

        public int MaxHp { get; private set; }                  // 런타임 실제 최대 HP (ApplyStats에서 갱신)
        public int CurrentHp { get; private set; }
        public bool IsInvincible { get; private set; }

        private float invincibleEndTime;

        public bool IsDead => CurrentHp <= 0;

        public void Init(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// 스탯 적용 시 최대 HP 갱신 + 현재 HP를 초과하지 않도록 조정
        /// </summary>
        public void ApplyStats(PlayerRuntimeStats playerStats, NecroRuntimeParams necroParams = null)
        {
            MaxHp = playerStats.maxHp;

            // 스탯 변경으로 maxHp가 줄어들었을 경우 현재 HP를 강제로 조정
            // (예: maxHp 200 → 150으로 내려가면 CurrentHp가 180이면 150으로 내려감)
            if (CurrentHp > MaxHp)
                CurrentHp = MaxHp;
        }

        public void ResetFull()
        {
            CurrentHp = MaxHp;
        }

        void Start()
        {
            // 초기값은 Awake에서 Stats가 로드된 후 ApplyStats로 결정되므로
            // Start에서 강제로 설정하지 않음 (ApplyStats가 호출될 것임)
            IsInvincible = false;
        }

        void Update()
        {
            if (IsInvincible && Time.time >= invincibleEndTime)
            {
                IsInvincible = false;
            }
        }

        public void Damaged(int damage)
        {
            if (damage <= 0) return;
            if (IsInvincible) return;
            if (IsDead) return;                     // 이미 죽은 상태면 추가 데미지 무시

            CurrentHp -= damage;
            if (CurrentHp < 0)
                CurrentHp = 0;

            IsInvincible = true;
            invincibleEndTime = Time.time + invincibleTime;

            OnDamaged(damage);

            if (CurrentHp <= 0)
            {
                Die();
            }
        }

        private void OnDamaged(int damage)
        {
            // 데미지 팝업 (항상 발생)
            GameManager.Instance.Pools.DamagePopups.Get(this.gameObject.transform.position + Vector3.up * 0.6f, damage, Color.red);
            if (player == null)
            {
                Debug.LogError("[PlayerHp] player 참조가 null입니다. Init(Player) 호출 확인!");
                return;
            }

            if (player.UI == null)
            {
                Debug.LogError("[PlayerHp] player.UI가 null입니다. Player.cs의 UI 참조 확인!");
                return;
            }

            if (player.UI.DamageFlashUI == null)
            {
                Debug.LogError("[PlayerHp] DamageFlashUI가 null입니다. PlayerUI 컴포넌트에서 할당 확인!");
                return;
            }
            // UI 플래시
            player?.UI?.DamageFlashUI?.Play();



            // 디버그 로그는 개발 중에만 남겨두고, 빌드 시 제거 고려
            Debug.Log($"Player damaged: -{damage}, HP={CurrentHp}/{MaxHp}");
        }

        private void Die()
        {
            // 사망 처리
            GameManager.Instance.GameOver();
            SaveManager.Instance.AddDeath(1);

            // 주의: 여기서 입력 차단, 애니메이션, 사운드 등을 추가해야 함
            // 현재는 최소한의 게임오버 트리거만 있음
        }

        // ==============================
        // 외부 API (치트 / 퍼크 / 회복용)
        // ==============================

        public void Heal(int value)
        {
            CurrentHp = Mathf.Min(CurrentHp + value, MaxHp);
        }

        public void FullHeal()
        {
            CurrentHp = MaxHp;
        }

        /// <summary>
        /// 디버그/치트용 강제 HP 설정
        /// 주의: 게임 로직에서 직접 호출하지 말고, 디버그 매니저 등에서만 사용
        /// </summary>
        public void SetHpDirect(int value)
        {
            CurrentHp = Mathf.Clamp(value, 0, MaxHp);
        }
    }
}