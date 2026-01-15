using UnityEngine;

namespace Necrogue.Enemy.Data
{

    [CreateAssetMenu(menuName = "Enemy/EnemyAI")]
    public class EnemyAIProfile : ScriptableObject
    {
        public float chaseRange;

        public float stopRange;
    }
}