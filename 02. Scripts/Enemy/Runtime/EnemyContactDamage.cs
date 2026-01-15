using UnityEngine;

using Necrogue.Common.Interfaces;

namespace Necrogue.Enemy.Runtime
{
    public class EnemyContactDamage : MonoBehaviour, IDamageSource
    {
        EnemyContext ctx;

        public int Damage
        {
            get
            {
                int dmg = ctx != null ? ctx.def.attack.contactDamage : 0;
                return dmg;
            }
        }
        public bool ConsumeOnHit => false;

        public void Init(EnemyContext ctx)
        {
            this.ctx = ctx;
        }

        public void Despawn()
        {
            // 몸박은 소멸하지 않음
        }
    }
}