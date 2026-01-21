using System.Collections.Generic;
using UnityEngine;

namespace Necrogue.Game.CombatUI
{
    public class UndeadExplosionPool : MonoBehaviour
    {
        [SerializeField] UndeadExplosion prefab;
        [SerializeField] int size = 5;

        readonly Queue<UndeadExplosion> q = new();
        bool initialized;

        public void Init()
        {
            if (initialized) return;
            initialized = true;

            for (int i = 0; i < size; i++)
                q.Enqueue(CreateOne());
        }

        public UndeadExplosion Get(Vector3 worldPos, float radius, int damage)
        {
            if (!initialized) Init();

            var vfx = q.Count > 0 ? q.Dequeue() : CreateOne();

            vfx.OwnerPool = this;
            vfx.transform.position = worldPos;
            vfx.transform.SetParent(transform, true);

            vfx.Setup(radius, damage);   // <-- 핵심
            vfx.gameObject.SetActive(true);
            vfx.Play();                  // Play()에서 DealDamageToEnemiesOnly() 호출

            return vfx;
        }


        UndeadExplosion CreateOne()
        {
            var vfx = Instantiate(prefab, transform);
            vfx.gameObject.SetActive(false);
            vfx.OwnerPool = this;               // 기본값
            return vfx;
        }

        public void Return(UndeadExplosion vfx)
        {
            if (!vfx) return;
            vfx.gameObject.SetActive(false);
            q.Enqueue(vfx);
        }
    }
}
