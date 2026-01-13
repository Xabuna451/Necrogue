using System;
using UnityEngine;

/// <summary>
/// PerkEffect가 강타입으로 값을 밀어넣는 수신자.
/// 값이 바뀌면 OnChanged로 알림.
/// </summary>
public class NecroPerkState : MonoBehaviour
{
    public event Action OnChanged;

    // Damage
    public float AllyDamageMul { get; private set; } = 1f;
    public float AllyDamageAdd { get; private set; } = 0f;

    // HP
    public float AllyHpMul { get; private set; } = 1f;
    public float AllyHpAdd { get; private set; } = 0f;

    // Cap
    public int AllyCapBonus { get; private set; } = 0;

    public void SetAllyDamage(float mul, float add)
    {
        float nm = Mathf.Max(0.01f, mul);
        float na = add;

        // 부동소수점(float) 비교 전용
        // 0.1f + 0.2f != 0.3f 같은 문제 방지
        if (Mathf.Approximately(AllyDamageMul, nm) && Mathf.Approximately(AllyDamageAdd, na)) return;
        AllyDamageMul = nm;
        AllyDamageAdd = na;

        Debug.Log($"[NecroPerkState] SetAllyDamage Mul={AllyDamageMul}, Add={AllyDamageAdd}");
        OnChanged?.Invoke();
    }

    public void SetAllyHp(float mul, float add)
    {
        float nm = Mathf.Max(0.01f, mul);
        float na = add;

        if (Mathf.Approximately(AllyHpMul, nm) && Mathf.Approximately(AllyHpAdd, na)) return;
        AllyHpMul = nm;
        AllyHpAdd = na;

        Debug.Log($"[NecroPerkState] SetAllyHp Mul={AllyHpMul}, Add={AllyHpAdd}");
        OnChanged?.Invoke();
    }

    public void SetAllyCapBonus(int bonus)
    {
        int nb = Mathf.Max(0, bonus);
        if (AllyCapBonus == nb) return;

        AllyCapBonus = nb;
        Debug.Log($"[NecroPerkState] AllyCapBonus -> {AllyCapBonus}");
        OnChanged?.Invoke();
    }
}
