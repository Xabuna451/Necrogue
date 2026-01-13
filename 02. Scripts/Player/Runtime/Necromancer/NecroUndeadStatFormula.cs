using UnityEngine;

/// <summary>
/// "계산"만 담당. Unity 오브젝트 상태/리스트/코루틴 모름.
/// </summary>
public static class NecroUndeadStatFormula
{
    public static int ComputeMaxHp(int baseMaxHp, float reviveHpMul, float perkHpAdd, float perkHpMulPercent)
    {
        int revivedBase = Mathf.Max(1, Mathf.RoundToInt(baseMaxHp * reviveHpMul));
        float v = (revivedBase + perkHpAdd) * (1f + perkHpMulPercent);
        return Mathf.Max(1, Mathf.RoundToInt(v));
    }


    /// <summary>
    /// EnemyCtrl이 SetAttackMul(float)만 가지므로,
    /// 
    /// </summary>
    public static float ComputeAttackMul(float baseAtk, float reviveAtkMul, float perkDmgAdd, float perkDmgMulPercent)
    {
        baseAtk = Mathf.Max(1f, baseAtk);

        float revivedBaseAtk = baseAtk * reviveAtkMul;
        float finalAtk = (revivedBaseAtk + perkDmgAdd) * (1f + perkDmgMulPercent);

        // EnemyCtrl이 배율만 받으니까 baseAtk 대비 배율로 환산
        float mul = finalAtk / baseAtk;
        return Mathf.Max(0.01f, mul);
    }

}
