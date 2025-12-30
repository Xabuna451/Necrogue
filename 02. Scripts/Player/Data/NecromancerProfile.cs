using UnityEngine;

[CreateAssetMenu(menuName = "Necromancer/Profile")]
public class NecromancerProfile : ScriptableObject
{
    [Header("거느릴 수 있는 소환수 개수")]
    public int maxCount = 1;

    [Header("네크로맨서 레벨 (적 내성보다 낮으면 소환 안됨)")]
    public int level = 1;

    [Header("부활 딜레이")]
    public float reviveDelay = 5f;

    [Header("부활 확률")]
    public float reviveChance = 0.25f;

    [Header("부활 스탯 계수")]
    [Range(0.05f, 1f)] public float hpMul = 0.5f;

    [Range(0.05f, 1f)] public float attackMul = 0.5f;

    [Header("제약")]
    public bool bossOK = false;
}
