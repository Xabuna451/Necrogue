using UnityEngine;
using Necrogue.Player.Runtime;
using Necrogue.Common.Interfaces;

namespace Necrogue.Player.Runtime
{
    public class PlayerHit : MonoBehaviour
    {
        [Header("피격 판정")]
        [SerializeField] private LayerMask attackedLayer;
        [SerializeField] private float hitRadius = 0.3f;
        [SerializeField] private Transform hitCheckCenter;

        private Collider2D[] hitBuffer = new Collider2D[32];

        private Player player;
        private PlayerHp hp;
        private PlayerMovement movement;

        public void Init(Player player)
        {
            this.player = player;
            this.hp = player?.Hp;
            this.movement = player?.Move;
        }

        // 주의: 현재는 Player.cs의 FixedUpdate에서 호출 중
        //      → 나중에 GameManager나 중앙 PhysicsManager로 옮길 것
        [System.Obsolete]
        public void CheckHit()
        {
            if (hp == null || hp.IsInvincible || hp.IsDead) return;

            Vector2 center = hitCheckCenter ? hitCheckCenter.position :
                             movement ? movement.Position :
                             (Vector2)transform.position;

            int count = Physics2D.OverlapCircleNonAlloc(
                center,
                hitRadius,
                hitBuffer,
                attackedLayer
            );

            if (count <= 0) return;

            int maxDamage = 0;

            // 이번 프레임 내 최대 데미지 1회만 적용
            for (int i = 0; i < count; i++)
            {
                var col = hitBuffer[i];
                if (!col) continue;

                var dmgSource = col.GetComponent<IDamageSource>() ?? col.GetComponentInParent<IDamageSource>();
                if (dmgSource == null) continue;

                int dmg = dmgSource.Damage;
                if (dmg > maxDamage) maxDamage = dmg;
            }

            if (maxDamage > 0)
            {
                hp.Damaged(maxDamage);
            }

            // ConsumeOnHit 처리 (투사체 소멸 등)
            for (int i = 0; i < count; i++)
            {
                var col = hitBuffer[i];
                if (!col)
                {
                    hitBuffer[i] = null;
                    continue;
                }

                var dmgSource = col.GetComponent<IDamageSource>() ?? col.GetComponentInParent<IDamageSource>();
                if (dmgSource != null && dmgSource.ConsumeOnHit)
                {
                    dmgSource.Despawn();
                }

                hitBuffer[i] = null;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Vector3 center = hitCheckCenter ? hitCheckCenter.position :
                             movement ? (Vector3)movement.Position :
                             transform.position;

            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawSphere(center, hitRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, hitRadius);
        }
#endif
    }
}