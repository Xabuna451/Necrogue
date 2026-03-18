using UnityEngine;
using Necrogue.Core.Domain.Necro;
using Necrogue.Player.Runtime;
using Necrogue.Enemy.Runtime;
using Necrogue.Game.Systems;

namespace Necrogue.Player.Runtime
{
    public class PlayerNecroController : MonoBehaviour
    {
        [SerializeField] private int necromancerLevel;
        public int Level => necromancerLevel;

        [SerializeField] private NecroPerkState perkState;
        [SerializeField] private InputManager input;
        [SerializeField] private float findRange = 5f;

        [Header("Layer")]
        [SerializeField] private LayerMask corpseLayer;   // 시체 (Corpse Faction)
        [SerializeField] private LayerMask undeadLayer;   // 언데드 (Ally Faction)

        private Player player;
        [SerializeField] private EnemyHp hoverCorpse;     // 현재 하이라이트 중인 가장 가까운 시체

        public void Init(Player p, InputManager im)
        {
            player = p;
            input = im;

            // 주의: Stats.necromaner가 null일 경우 기본값 1로 안전 처리
            necromancerLevel = player?.Stats?.necromaner?.level ?? 1;
        }

        void Update()
        {
            FindNearestCorpse();

            if (input?.RightClick == true)
                KillUndeadUnderMouse();
        }

        /// <summary>
        /// 퍼크/스탯 변경 시 NecroPerkState에 값 전달 후 모든 언데드 재계산
        /// </summary>
        public void ApplyRuntime(NecroRuntimeParams p)
        {
            if (!perkState || p == null) return;

            // NecroPerkState에 직접 값 밀어넣기
            perkState.SetAllyDamage(p.allyDamageMul, p.allyDamageAdd);
            perkState.SetAllyHp(p.allyHpMul, p.allyHpAdd);
            perkState.SetAllyCapBonus(p.allyCapAdd);

            // 즉시 반영: 현재 존재하는 모든 언데드 스탯 재계산
            RebuildAllUndeadStats();
        }

        private void RebuildAllUndeadStats()
        {
            // 100f는 임시로 충분히 큰 범위 -> 나중에 실제 맵 크기나 QuadTree 등으로 최적화 고려
            var hits = Physics2D.OverlapCircleAll(
                player.transform.position,
                100f,
                undeadLayer
            );

            foreach (var h in hits)
            {
                var ctrl = h.GetComponentInParent<EnemyContext>();
                var hp = h.GetComponentInParent<EnemyHp>();

                if (!ctrl || !hp) continue;
                if (ctrl.Faction != Faction.Ally) continue;

                ApplyUndeadStat(ctrl, hp);
            }
        }

        private void ApplyUndeadStat(EnemyContext ctrl, EnemyHp hp)
        {
            var necroProfile = player?.Stats?.necromaner;
            if (!necroProfile) return;

            // HP 계산 및 적용
            int maxHp = NecroUndeadStatFormula.ComputeMaxHp(
                hp.Hp,                      // EnemyStatAsset 기준 base HP
                necroProfile.hpMul,
                perkState.AllyHpAdd,
                perkState.AllyHpMul - 1f    // Mul은 1-based (1.2 = +20%)
            );

            hp.SetMaxHp(maxHp);

            // Attack 배율 계산 및 적용
            float atkMul = NecroUndeadStatFormula.ComputeAttackMul(
                ctrl.def.attack.attackDamage,   // base attack
                necroProfile.attackMul,
                perkState.AllyDamageAdd,
                perkState.AllyDamageMul - 1f    // Mul은 1-based
            );

            ctrl.SetAttackMul(atkMul);
        }

        private void FindNearestCorpse()
        {
            if (!player) return;

            Vector2 pos = player.transform.position;
            var hits = Physics2D.OverlapCircleAll(pos, findRange, corpseLayer);

            EnemyHp best = null;
            float bestDistSqr = float.MaxValue;

            foreach (var h in hits)
            {
                var hp = h.GetComponentInParent<EnemyHp>();
                if (!hp) continue;

                var ctrl = hp.GetComponentInParent<EnemyContext>();
                if (!ctrl || ctrl.Faction != Faction.Corpse) continue;

                float distSqr = (hp.transform.position - player.transform.position).sqrMagnitude;
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    best = hp;
                }
            }
            
            hoverCorpse = best;
        }

        /// <summary>
        /// 마우스 위치의 언데드 폭파 (현재 하드코딩된 값 사용)
        /// 나중에는 퍼크/스탯에서 radius, damage 가져오도록 변경 예정
        /// </summary>
        private void KillUndeadUnderMouse()
        {
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var hit = Physics2D.Raycast(mouse, Vector2.zero, 0f, undeadLayer);
            if (!hit.collider) return;

            var hp = hit.collider.GetComponentInParent<EnemyHp>();
            var ctrl = hit.collider.GetComponentInParent<EnemyContext>();
            if (!hp || !ctrl) return;

            if (ctrl.Faction != Faction.Ally) return;

            Vector3 pos = hp.transform.position;

            // 임시 하드코딩 값 → 나중에 NecroRuntimeParams나 Perk에서 동적 가져오기
            float radius = 2.5f;
            int damage = 1557;

            var gm = GameManager.Instance;
            if (gm?.Pools != null)
            {
                gm.Pools.UndeadExplosions?.Get(pos, radius, damage);
            }

            hp.Damaged(999999);
        }

        // ==============================
        // 외부 API (레벨 조작)
        // ==============================
        public void NecromancerLevelUp()
        {
            necromancerLevel++;
        }

        public void NecromancerLevelDown()
        {
            necromancerLevel = Mathf.Max(1, necromancerLevel - 1);
        }
    }
}