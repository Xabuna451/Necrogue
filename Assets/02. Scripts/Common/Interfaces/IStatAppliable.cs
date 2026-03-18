using Necrogue.Core.Domain.Stats;
using Necrogue.Core.Domain.Necro;

public interface IStatAppliable
{
    /// <summary>
    /// 런타임 스탯을 적용받는 메서드.
    /// - playerStats: 일반 스탯 (maxHp, attack, speed 등)
    /// - necroParams: 네크로 관련 스탯 (선택적, null 가능)
    /// </summary>
    /// <remarks>
    /// 각 컴포넌트에서 필요한 스탯만 사용. (e.g., Movement는 speed만)
    /// </remarks>
    void ApplyStats(PlayerRuntimeStats playerStats, NecroRuntimeParams necroParams = null);
}