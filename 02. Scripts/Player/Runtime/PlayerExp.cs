using UnityEngine;
using System;

using Necrogue.Player.Runtime;

public class PlayerExp : MonoBehaviour
{
    private Player player;
    public void Init(Player player) => this.player = player;

    [SerializeField] private int level = 1;
    public int Level => level;

    [Header("EXP 설정")]
    [SerializeField] private float maxExp = 100f;
    [SerializeField] private float currentExp = 0f;

    public float Exp => currentExp;
    public float MaxExp => maxExp;

    [Header("레벨 곡선")]
    [SerializeField] private float growthMul = 1.1f;   // 여기만 바꾸면 됨
    [SerializeField] private int maxLevel = 50;

    public event Action<int> OnLeveledUp; // 특전 UI는 여기 구독해서 열면 됨

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= maxExp && level < maxLevel)
        {
            currentExp -= maxExp;
            LevelUp();
        }

        // 만렙이면 초과분 처리(버림/고정/저장) 중 택1
        if (level >= maxLevel)
            currentExp = Mathf.Min(currentExp, maxExp - 0.0001f);
    }

    private void LevelUp()
    {
        level++;
        maxExp *= growthMul;

        OnLeveledUp?.Invoke(level);
    }

    // =================================================================
    // 디버그

#if UNITY_EDITOR
    public void DebugAddExp(int exp)
    {
        AddExp(exp);
    }

    [ContextMenu("Level Up")]
    public void DebugLevelUp()
    {
        LevelUp();
    }

#endif

}