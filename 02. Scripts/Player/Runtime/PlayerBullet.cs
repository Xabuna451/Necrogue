using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBullet : MonoBehaviour
{
    Rigidbody2D rb;

    int damage;
    float lifeTime;
    float spawnTime;

    public PlayerBulletPool OwnerPool;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetForSpawn()
    {
        damage = 0;
        lifeTime = 0f;
        spawnTime = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // 너 환경 기준
            rb.angularVelocity = 0f;
        }
    }

    public void Fire(Vector2 dir, float speed, int damage, float lifeTime)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        spawnTime = Time.time;

        rb.linearVelocity = dir * speed;
    }

    void Update()
    {
        if (Time.time - spawnTime >= lifeTime)
        {
            Despawn();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemyHp = other.GetComponentInParent<EnemyHp>();
        if (enemyHp != null)
        {
            enemyHp.Damaged(damage);
        }

        Despawn();
    }

    void Despawn()
    {
        if (OwnerPool != null)
            OwnerPool.Return(this);
        else
            gameObject.SetActive(false); // 풀 없으면 최소 안전 처리
    }
}
