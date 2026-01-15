using UnityEngine;

using Necrogue.Player.Runtime;

using Necrogue.Game.Systems;

using Necrogue.Weapon.Runtime;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform firePoint;

    [Header("무기 데이터(SO)")]
    [SerializeField] private WeaponProfile weapon;

    float nextFireTime;

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
        if (!player) return;
        if (!firePoint) return;
        if (!weapon) return;

        var pools = GameManager.Instance ? GameManager.Instance.Pools : null;
        var pool = pools ? pools.Bullets : null;
        if (!pool) return;

        if (Time.time < nextFireTime) return;

        FireMouseDirection(pool);
        nextFireTime = Time.time + Mathf.Max(0.01f, weapon.fireInterval);
    }

    void FireMouseDirection(PlayerBulletPool pool)
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

        int dmg = player.Attack != null ? player.Attack.Attack : 1;

        // “무기값을 주입”하는 방식으로 다양화
        bullet.Fire(dir, weapon.bulletSpeed, dmg, weapon.bulletLifeTime);
    }
}
