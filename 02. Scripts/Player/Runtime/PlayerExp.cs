using UnityEngine;

public class PlayerExp : MonoBehaviour
{
    private Player player;

    public void Init(Player player)
    {
        this.player = player;
    }

    [SerializeField] private int level;
    public int Level => level;

    [Header("EXP 설정")]
    private float maxExp = 100f;

    [SerializeField] private float currentExp = 0;
    public float Exp => currentExp;
    public float MaxExp => maxExp;

    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"Player gained {amount} EXP. Total EXP: {currentExp}");

        if (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        Debug.Log("Player leveled up!");
        level++;

        maxExp *= 1.25f;

        // 레벨업 추가 효과 (특전 선택)
    }

    // =================================================================
    // 디버그

#if UNITY_EDITOR
    [ContextMenu("Add 50 EXP")]
    private void DebugAddExp()
    {
        AddExp(50);
    }

    [ContextMenu("Level Up")]
    private void DebugLevelUp()
    {
        LevelUp();
    }

#endif




}