using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDirectMove : MonoBehaviour, IEnemyMove
{
    EnemyCtrl ctx;
    Rigidbody2D rb;
    [Header("공격 콜라이더 방향 전환")]
    [SerializeField] Collider2D col;

    Vector2 target;
    bool hasTarget;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(EnemyCtrl ctx)
    {
        this.ctx = ctx;
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    public void MoveTo(Vector2 target)
    {
        this.target = target;
        hasTarget = true;
        ctx?.Animation?.SetMove(true);
    }

    public void Stop()
    {
        hasTarget = false;
        if (rb) rb.linearVelocity = Vector2.zero;
        ctx?.Animation?.SetMove(false);
    }

    void FixedUpdate()
    {
        if (!hasTarget) return;

        if (ctx == null || ctx.def == null || ctx.def.stats == null) return;

        Vector2 dir = target - rb.position;
        if (dir.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        dir.Normalize();
        rb.linearVelocity = dir * ctx.def.stats.moveSpeed;

        if (ctx.spriteRenderer)
        {
            ctx.spriteRenderer.flipX = dir.x < 0f;
        }
        if (col)
        {
            var o = col.offset;
            o.x = dir.x < 0f ? -0.18f : 0.1f;
            col.offset = o;
        }
    }
}