using UnityEngine;
namespace Necrogue.Enemy.Data
{
    [CreateAssetMenu(menuName = "Enemy/EnemyStats")]
    public class EnemyStatAsset : ScriptableObject
    {
        [Header("최대 체력")]
        public int maxHp;

        [Header("이동 속도")]

        public float moveSpeed;

        [Header("언데드 내성 레벨 (네크로맨서의 레벨보다 높다면 소환 X)")]
        public int underLevel;
    }
}