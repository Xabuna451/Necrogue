using UnityEngine;
namespace Necrogue.Enemy.Data
{
    [CreateAssetMenu(menuName = "Enemy/EnemyAttack")]
    public class EnemyAttackProfile : ScriptableObject
    {
        [Header("몸통 접촉 데미지")]
        public int contactDamage;   // 몸통 접촉 데미지

        [Header("공격 데미지")]
        public int attackDamage;    // 공격 데미지

        [Header("공격 사정거리")]
        public float attackRange;   // 공격 사정거리
        [Header("공격 속도")]
        public float attackRate;   // 공격 속도
    }
}