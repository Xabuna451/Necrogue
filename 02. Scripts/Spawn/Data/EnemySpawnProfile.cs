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

        [Header("엘리트 변형(없으면 null)")]
        public EnemyEliteProfile elite;
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

        [Header("엘리트 설정")]
        [Range(0f, 1f)] public float eliteChance = 0.05f;
        public int maxEliteAlive = 2;

        [Header("이 구간에서 등장 가능한 적(가중치)")]
        public Entry[] table;
    }

    [Serializable]
    public struct PickResult
    {
        public EnemyDefAsset def;
        public EnemyEliteProfile elite; // null이면 일반
        public bool IsElite => elite != null;
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

    public PickResult Pick(Stage s, int eliteAlive)
    {
        if (s == null || s.table == null || s.table.Length == 0)
            return default;

        // 1) 가중치로 원본 몬스터 선택 (Entry를 반환해야 elite도 같이 알 수 있음)
        int sum = 0;
        for (int i = 0; i < s.table.Length; i++)
            sum += Mathf.Max(0, s.table[i].weight);

        if (sum <= 0) return default;

        int r = UnityEngine.Random.Range(0, sum);
        int acc = 0;

        Entry picked = s.table[s.table.Length - 1];
        for (int i = 0; i < s.table.Length; i++)
        {
            acc += Mathf.Max(0, s.table[i].weight);
            if (r < acc) { picked = s.table[i]; break; }
        }

        // 2) 엘리트 판정 (확률 + 제한 + elite 프로필 존재)
        bool canElite = (picked.elite != null) && (eliteAlive < s.maxEliteAlive);
        bool rollElite = UnityEngine.Random.value < s.eliteChance;

        return new PickResult
        {
            def = picked.def,
            elite = (canElite && rollElite) ? picked.elite : null
        };
    }

}
