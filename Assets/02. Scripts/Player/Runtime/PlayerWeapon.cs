using UnityEngine;
using Necrogue.Player.Runtime;
using Necrogue.Game.Systems;
using Necrogue.Weapon.Runtime;

namespace Necrogue.Player.Runtime
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private Transform firePoint;

        [Header("무기 데이터(SO)")]
        [SerializeField] private WeaponProfile weapon;

        private float nextFireTime;

        public void Init(Player player)
        {
            this.player = player;
        }

        public void SetWeapon(WeaponProfile weapon)
        {
            if (!weapon) return;
            this.weapon = weapon;
            nextFireTime = 0f;
        }

        void Update()
        {
            if (!player || !firePoint || !weapon) return;

            var pools = GameManager.Instance?.Pools;
            var pool = pools?.Bullets;
            if (!pool) return;

            if (Time.time < nextFireTime) return;

            FireMouseDirection(pool);
            nextFireTime = Time.time + Mathf.Max(0.01f, weapon.fireInterval);
        }

        private void FireMouseDirection(PlayerBulletPool pool)
        {
            var cam = Camera.main;
            if (!cam) return;

            Vector3 mp = Input.mousePosition;
            mp.z = -cam.transform.position.z;
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mp);
            mouseWorld.z = 0f;

            Vector2 dir = (Vector2)mouseWorld - (Vector2)firePoint.position;
            if (dir.sqrMagnitude < 0.0001f) return;
            dir.Normalize();

            var bullet = pool.GetBullet();
            if (!bullet) return;

            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.identity;

            // 주의: player.Attack이 null일 경우 기본값 1 사용
            // 나중에 PlayerAttack 컴포넌트가 제거/변경될 가능성 대비
            int dmg = player?.Attack?.Attack ?? 1;

            bullet.Fire(dir, weapon.bulletSpeed, dmg, weapon.bulletLifeTime);
        }
    }
}