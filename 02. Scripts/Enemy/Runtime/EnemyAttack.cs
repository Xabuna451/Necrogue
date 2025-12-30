using UnityEngine;

public class EnemyAttack : MonoBehaviour, IEnemyAttack
{
    EnemyCtrl ctx;

    [Header("공격 판정용 콜라이더")]
    public Collider2D col;
    float nextAttackTime;

    public void Init(EnemyCtrl ctx)
    {
        this.ctx = ctx;
        nextAttackTime = 0f;
        col.isTrigger = true;
    }

    public void Tick()
    {
        if (ctx == null || ctx.def == null) return;
        if (ctx.Target == null) return;

        var profile = ctx.def.attack;
        if (profile == null) return;

        float dist = Vector2.Distance(transform.position, ctx.Target.position);
        if (dist > profile.attackRange) return;

        if (profile.attackRate <= 0f) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + (1f / profile.attackRate);

        // (아군/적군 여부는 "Retarget이 상대 진영을 잡는다"는 전제)
        var playerHp = ctx.Target.GetComponentInParent<PlayerHp>();
        if (playerHp != null)
        {
            ctx.Animation?.PlayAttack();

            // 나중에 콜라이더로 접촉 판정 넣을 수도 있으니 일단 직접 대미지 주기
            playerHp.Damaged(profile.attackDamage);

            return;
        }

        var enemyHp = ctx.Target.GetComponentInParent<EnemyHp>();
        if (enemyHp != null)
        {
            ctx.Animation?.PlayAttack();

            // 나중에 콜라이더로 접촉 판정 넣을 수도 있으니 일단 직접 대미지 주기
            enemyHp.Damaged(profile.attackDamage);

            return;
        }
    }

    void OnTriggerEnter2D()
    {
        if (col.CompareTag("Ally") || col.CompareTag("Enemy"))
        {
            ctx.DirMove.isAttack = true;
        }
    }

}
