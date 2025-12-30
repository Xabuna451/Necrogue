using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletPool : MonoBehaviour
{
    [Header("런타임 초기화됨")]
    public PlayerBullet bulletPrefab;
    public int initialSize = 100;

    readonly Queue<PlayerBullet> pool = new Queue<PlayerBullet>();
    bool initialized;

    public void Init(PlayerBullet prefab, int size)
    {
        if (initialized) return;

        bulletPrefab = prefab;
        initialSize = size;

        if (!bulletPrefab)
        {
            Debug.LogError("[PlayerBulletPool] bulletPrefab 비어 있음");
            return;
        }

        for (int i = 0; i < initialSize; i++)
            CreateNewBullet();

        initialized = true;
    }

    PlayerBullet CreateNewBullet()
    {
        var bullet = Instantiate(bulletPrefab, transform);
        bullet.gameObject.SetActive(false);
        bullet.OwnerPool = this;
        pool.Enqueue(bullet);
        return bullet;
    }

    public PlayerBullet GetBullet()
    {
        if (!initialized)
        {
            Debug.LogError("[PlayerBulletPool] Init을 먼저 호출해야 함");
            return null;
        }

        if (pool.Count == 0)
            CreateNewBullet();

        var bullet = pool.Dequeue();
        bullet.gameObject.SetActive(true);
        bullet.ResetForSpawn();
        return bullet;
    }

    public void Return(PlayerBullet bullet)
    {
        if (!bullet) return;
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
    }
}
