using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/FactionVisualConfig", fileName = "FactionVisualConfig_Default")]
public class FactionVisualConfig : ScriptableObject
{
    [Header("기본 팩션 색상")]
    public Color EnemyColor = new Color(1f, 1f, 1f, 1f);    // white
    public Color AllyColor = new Color(0f, 1f, 1f, 1f);    // cyan
    public Color CorpseColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);

    [Header("특수 상태 오버레이 색상")]
    public Color EliteColor = new Color(1f, 0.4f, 0.2f, 1f);   // 주황빛 강한 느낌
    public Color ResurrectionGlow = new Color(0f, 0.9f, 1f, 0.85f);  // 부활 시 빛나는 청록

    [Header("추가 조정")]
    [Range(0.3f, 1f)] public float CorpseAlphaMultiplier = 0.7f;
}