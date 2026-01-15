using UnityEngine;

using Necrogue.Enemy.Runtime;
using Necrogue.Common.Interfaces;

namespace Necrogue.Enemy.Data.States
{
    public class AttackState : EnemyState
    {
        private float nextAttackTime;

        public AttackState(EnemyContext ctx) : base(ctx)
        {
            nextAttackTime = 0f;
        }

        public override void Enter()
        {
            ctx.Move.Stop();
            ctx.Animation?.SetMove(false);
            nextAttackTime = 0f;
        }

        public override void Tick()
        {
            if (ctx.Target == null || !ctx.IsValidTarget(ctx.Target))
            {
                ctx.StateMachine.SwitchState(EnemyStateType.Idle);
                return;
            }

            // 안전장치: def, attack 확인
            if (ctx.def == null || ctx.def.attack == null)
            {
                Debug.LogError($"[AttackState] {ctx.name} - Missing def or attack profile!");
                ctx.StateMachine.SwitchState(EnemyStateType.Idle);
                return;
            }

            float dist = Vector2.Distance(ctx.transform.position, ctx.Target.position);

            // 공격 범위 벗어나면 다시 추격
            if (dist > ctx.def.attack.attackRange)
            {
                ctx.StateMachine.SwitchState(EnemyStateType.Chase);
                return;
            }

            // 쿨타임 체크
            if (Time.time < nextAttackTime) return;

            // 공격 실행
            nextAttackTime = Time.time + (1f / ctx.def.attack.attackRate);

            ctx.Animation?.PlayAttack();

            // AttackMul 적용 (네크로맨서 부활 시 공격력 감소)
            int baseDamage = ctx.def.attack.attackDamage;
            int finalDamage = Mathf.RoundToInt(baseDamage * ctx.AttackMul);

            // IDamageable로 통일된 데미지 처리
            var damageable = ctx.Target.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damaged(finalDamage);
            }
            else
            {
                Debug.LogWarning($"[AttackState] {ctx.name} - Target {ctx.Target.name} has no IDamageable!");
            }
        }
    }
}