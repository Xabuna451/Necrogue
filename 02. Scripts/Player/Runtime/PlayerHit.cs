using UnityEngine;

using Necrogue.Player.Runtime;

public class PlayerHit : MonoBehaviour
{
    [Header("피격 판정")]
    [SerializeField] private LayerMask attackedLayer;
    [SerializeField] private float hitRadius = 0.3f;
    [SerializeField] private Transform hitCheckCenter;

    private Collider2D[] hitBuffer = new Collider2D[32];

    Player player;
    PlayerHp hp;
    PlayerMovement movement;

    public void Init(Player player)
    {
        this.player = player;
        this.hp = player?.Hp;
        this.movement = player?.Move;
    }

    // Player.Update()에서 호출
    [System.Obsolete]
    public void CheckHit()
    {
        if (hp == null) return;
        if (hp.IsInvincible || hp.CurrentHp <= 0) return;

        Vector2 center;
        if (hitCheckCenter != null)
            center = hitCheckCenter.position;
        else if (movement != null)
            center = movement.Position;
        else
            center = transform.position;

        int count = Physics2D.OverlapCircleNonAlloc(
            center,
            hitRadius,
            hitBuffer,
            attackedLayer
        );

        if (count <= 0) return;

        int maxDamage = 0;

        // 이번 프레임에 맞은 것 중 "최대 데미지 1회만"
        for (int i = 0; i < count; i++)
        {
            var col = hitBuffer[i];
            if (col == null) continue;

            var dmgSource = col.GetComponent<IDamageSource>() ?? col.GetComponentInParent<IDamageSource>();

            if (dmgSource == null) continue;

            int dmg = dmgSource.Damage;
            if (dmg > maxDamage)
                maxDamage = dmg;
        }

        if (maxDamage > 0)
        {
            hp.Damaged(maxDamage);
        }

        // ConsumeOnHit 처리
        for (int i = 0; i < count; i++)
        {
            var col = hitBuffer[i];
            if (col == null)
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
        Vector3 center;
        if (hitCheckCenter != null)
            center = hitCheckCenter.position;
        else
            center = transform.position;

        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(center, hitRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, hitRadius);
    }
#endif
}
