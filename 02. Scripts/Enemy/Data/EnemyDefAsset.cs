using UnityEngine;

using Necrogue.Enemy.Data;
namespace Necrogue.Enemy.Data
{
    [CreateAssetMenu(menuName = "Enemy/EnemyDef")]
    public class EnemyDefAsset : ScriptableObject
    {
        [Header("ID")]
        public string id;
        public string displayName;
        public string description;

        [Header("프리팹")]
        public GameObject enemyPrefab;

        [Header("스탯")]
        public EnemyStatAsset stats;

        [Header("공격")]
        public EnemyAttackProfile attack;
        [Header("적 AI")]
        public EnemyAIProfile ai;

        [Header("비주얼")]
        public FactionVisualConfig visual;

        [Header("보상")]
        public EnemyRewardAsset reward;
        [Header("엘리트 몬스터")]
        public bool isElite;
        public EnemyEliteProfile eliteDef;
        [Header("Boss (잡몹이면 null)")]
        public BossModuleAsset boss; // 잡몹이면 null

        [Header("타겟 셀렉터")]
        public TargetSelector targetSelector;
    }
}