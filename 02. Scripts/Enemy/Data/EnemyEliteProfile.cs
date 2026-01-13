using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyEliteProfile")]
public class EnemyEliteProfile : ScriptableObject
{
    public EnemyDefAsset original;

    [Header("외형")]
    public Color color = Color.yellow;
    public float scaleMul = 1.15f;

    [Header("스탯 배율")]
    public float hpMul = 2.5f;
    public float atkMul = 1.5f;
    public float moveMul = 1.1f;

    [Header("보상 배율")]
    public float rewardMul = 2.0f;
}
