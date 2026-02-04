using UnityEngine;
using Necrogue.Core.Domain.Stats;
using Necrogue.Player.Runtime;

using Necrogue.Core.Domain.Necro;

namespace Necrogue.Player.Runtime
{
    public class PlayerAttack : MonoBehaviour, IStatAppliable
    {
        [Header("공격 스탯")]
        [SerializeField] private int baseAttack = 10;           // 에디터 기본값 (스탯 미적용 시)

        private int currentAttack;                              // 런타임 실제 공격력

        private Player player;

        public int Attack => currentAttack;

        public void Init(Player player)
        {
            this.player = player;
        }

        public void ApplyStats(PlayerRuntimeStats playerStats, NecroRuntimeParams necroParams = null)
        {
            currentAttack = playerStats.attack;

            // attack이 0 이하로 내려가지 않도록 최소값 보장
            if (currentAttack < 1) currentAttack = 1;
        }

        // ==============================================
        // 디버그 / 치트용 API
        // ==============================================
        /// <summary>
        /// 강제 공격력 설정 (디버그/치트용)
        /// 주의: 게임 로직에서 직접 호출하지 말고 DebugManager 등에서만 사용
        /// </summary>
        public void SetAttackDirect(int value)
        {
            currentAttack = Mathf.Max(1, value);
            Debug.Log($"[DEBUG] Player Attack set to {currentAttack}");
        }
    }
}