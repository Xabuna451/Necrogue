using UnityEngine;
using System;

using Necrogue.Game.Sounds;
using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Necro;

namespace Necrogue.Player.Runtime
{
    public class PlayerExp : MonoBehaviour, IStatAppliable
    {
        private Player player;
        public void Init(Player player) => this.player = player;

        [SerializeField] private int baseLevel = 1;
        private int currentLevel;

        public int Level => currentLevel;

        [Header("EXP 설정")]
        [SerializeField] private float baseMaxExp = 100f;
        private float currentMaxExp;

        [SerializeField] private float currentExp = 0f;

        public float Exp => currentExp;
        public float MaxExp => currentMaxExp;

        [Header("레벨 곡선")]
        [SerializeField] private float growthMul = 1.1f;   // 곡선 변경 시 이 값만 조정
        [SerializeField] private int maxLevel = 50;

        public event Action<int> OnLeveledUp;

        public void ApplyStats(PlayerRuntimeStats playerStats, NecroRuntimeParams necroParams = null)
        {
            // 현재는 PlayerRuntimeStats에 exp 관련 필드가 없으므로 적용 대상 없음
            // 나중에 exp 곡선/초기값/보너스 등이 스탯에 들어오면 여기서 갱신
            // 예: currentMaxExp = playerStats.baseExp * growthMul 등
        }

        void Awake()
        {
            // 초기값 설정
            currentLevel = baseLevel;
            currentMaxExp = baseMaxExp;
        }

        public void AddExp(int amount)
        {
            if (amount <= 0) return;

            currentExp += amount;

            while (currentExp >= currentMaxExp && currentLevel < maxLevel)
            {
                currentExp -= currentMaxExp;
                LevelUp();
            }

            // 만렙 초과분 처리: 현재는 maxExp 미만으로 제한
            // 다른 방식(누적 저장 등)이 필요하면 여기서 분기
            if (currentLevel >= maxLevel)
            {
                currentExp = Mathf.Min(currentExp, currentMaxExp - 0.0001f);
            }
        }

        private void LevelUp()
        {
            currentLevel++;

            // 주의: growthMul이 1.0 이하일 경우 maxExp가 줄어들 수 있음
            // (레벨 다운그레이드 방지하려면 Mathf.Max 사용 고려)
            currentMaxExp *= growthMul;

            SoundManager.Instance.PlaySFX(0);

            OnLeveledUp?.Invoke(currentLevel);
        }

        // ==============================================
        // 디버그 / 치트용 API
        // ==============================================
        /// <summary>
        /// 디버그용 경험치 추가 (직접 AddExp 호출)
        /// </summary>
        public void DebugAddExp(int exp)
        {
            AddExp(exp);
        }

        /// <summary>
        /// 컨텍스트 메뉴로 레벨업 강제 실행
        /// 주의: 한 번에 여러 레벨업을 할 경우 정상 동작 안함.
        /// 퍼크 선택지는 초기화 되지만 경험치는 다음 레벨 
        /// 음 오히려 정상 동작이긴하네.
        /// </summary>
        [ContextMenu("Force Level Up")]
        public void DebugLevelUp()
        {
            LevelUp();
        }
    }
}