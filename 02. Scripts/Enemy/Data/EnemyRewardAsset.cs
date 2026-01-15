using UnityEngine;
namespace Necrogue.Enemy.Data
{
    [CreateAssetMenu(menuName = "Enemy/Reward", fileName = "EnemyReward_")]
    public class EnemyRewardAsset : ScriptableObject
    {
        [Min(0)] public int minExp = 0;
        [Min(0)] public int maxExp = 0;

        [Min(0)] public int minGold = 0;
        [Min(0)] public int maxGold = 0;

        [Min(0)] public int eliteBonus = 0; // 엘리트면 추가 보너스(exp든 gold든 정책으로)

        public int RollExp(bool isElite)
        {
            int baseExp = Random.Range(minExp, maxExp + 1);
            return isElite ? baseExp + eliteBonus : baseExp;
        }

        public int RollGold(bool isElite)
        {
            int baseGold = Random.Range(minGold, maxGold + 1);
            return isElite ? baseGold + eliteBonus : baseGold;
        }

        void OnValidate()
        {
            if (maxExp < minExp) maxExp = minExp;
            if (maxGold < minGold) maxGold = minGold;
        }
    }
}