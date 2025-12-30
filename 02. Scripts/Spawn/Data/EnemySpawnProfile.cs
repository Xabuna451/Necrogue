using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawn/SpawnProfile")]
public class EnemySpawnProfile : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public EnemyDefAsset def;
        [Min(1)] public int weight; // 가중치
    }

    [Serializable]
    public class Stage
    {
        [Header("이 시간(초)부터 적용")]
        public float time;

        [Header("스폰 간격(초)")]
        public float delay;

        [Header("동시 존재 제한")]
        public int maxAlive;

        [Header("이 구간에서 등장 가능한 적(가중치)")]
        public Entry[] table;
    }

    [Header("시간순 정렬 필수 (0부터)")]
    public Stage[] stages;

    public Stage GetStage(float t)
    {
        if (stages == null || stages.Length == 0) return null;

        Stage current = stages[0];
        for (int i = 1; i < stages.Length; i++)
        {
            if (t < stages[i].time) break;
            current = stages[i];
        }
        return current;
    }

    public EnemyDefAsset Pick(Stage s)
    {
        if (s == null || s.table == null || s.table.Length == 0) return null;

        int sum = 0;
        for (int i = 0; i < s.table.Length; i++)
            sum += Mathf.Max(0, s.table[i].weight);

        if (sum <= 0) return null;

        int r = UnityEngine.Random.Range(0, sum);
        int acc = 0;

        for (int i = 0; i < s.table.Length; i++)
        {
            acc += Mathf.Max(0, s.table[i].weight);
            if (r < acc) return s.table[i].def;
        }

        return s.table[s.table.Length - 1].def;
    }
}
