using UnityEngine;

using Necrogue.Game.CombatUI;

using Necrogue.Enemy.Runtime;


public class UndeadExplosion : MonoBehaviour
{
    [Header("Explosion Damage")]
    [SerializeField] float radius = 2.5f;
    [SerializeField] int damage = 80;

    [Header("Hit Filter")]
    [SerializeField] LayerMask enemyLayer;   // Enemy 레이어만 지정(권장)

    public UndeadExplosionPool OwnerPool { get; set; }

    public void Setup(float r, int d)
    {
        radius = Mathf.Max(0f, r);
        damage = Mathf.Max(0, d);
    }
    public void Play()
    {
        // VFX 애니메이션 재생은 네가 이미 처리했다고 했으니 생략 가능
        // animator.Play(0, 0, 0f);

        DealDamageToEnemiesOnly();
    }

    void DealDamageToEnemiesOnly()
    {
        // 폭발 중심은 이 VFX 위치
        Vector2 center = transform.position;

        // 적 레이어만 긁기(성능/안전)
        var hits = Physics2D.OverlapCircleAll(center, radius, enemyLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (!col) continue;

            // 너 코드 기준: EnemyHp / EnemyContext 사용
            var hp = col.GetComponentInParent<EnemyHp>();
            var ctx = col.GetComponentInParent<EnemyContext>();
            if (!hp || !ctx) continue;

            // "적만"
            if (ctx.Faction != Faction.Enemy) continue;

            hp.Damaged(damage);
        }
    }

    // 애니메이션 마지막 프레임 Animation Event
    public void ExplodeAnimationEvent()
    {
        if (OwnerPool != null) OwnerPool.Return(this);
        else gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
