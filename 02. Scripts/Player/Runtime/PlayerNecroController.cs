using UnityEngine;
using Necrogue.Core.Domain.Necro;
using Necrogue.Player.Runtime;

using Necrogue.Enemy.Runtime;

namespace Necrogue.Player.Runtime
{
    public class PlayerNecroController : MonoBehaviour
    {
        [SerializeField] int necromancerLevel;
        public int Level => necromancerLevel;

        [SerializeField] NecroPerkState perkState;
        [SerializeField] InputManager input;
        [SerializeField] float findRange = 5f;

        [Header("Layer")]
        [SerializeField] LayerMask corpseLayer;   // 시체
        [SerializeField] LayerMask undeadLayer;   // 언데드(Ally)

        Player player;
        [SerializeField] EnemyHp hoverCorpse;   // 가장 가까운 시체

        public void Init(Player p, InputManager im)
        {
            player = p;
            input = im;
            necromancerLevel = player.Stats.necromaner.level;
        }
        void Update()
        {
            FindNearestCorpse();

            if (input.RightClick)
                KillUndeadUnderMouse();
        }

        public void ApplyRuntime(NecroRuntimeParams p)
        {
            if (!perkState || p == null) return;

            // 1) NecroPerkState에 값 밀어넣기
            perkState.SetAllyDamage(p.allyDamageMul, p.allyDamageAdd);
            perkState.SetAllyHp(p.allyHpMul, p.allyHpAdd);
            perkState.SetAllyCapBonus(p.allyCapAdd);

            // 2) 즉시 반영: 현재 언데드 전부 재계산
            RebuildAllUndeadStats();
        }
        void RebuildAllUndeadStats()
        {
            var hits = Physics2D.OverlapCircleAll(
                player.transform.position,
                100f,              // 전체 범위(충분히 크게)
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
        void ApplyUndeadStat(EnemyContext ctrl, EnemyHp hp)
        {
            var necroProfile = player.Stats.necromaner;
            if (!necroProfile) return;

            // === HP ===
            int maxHp = NecroUndeadStatFormula.ComputeMaxHp(
                hp.Hp,                 // EnemyStatAsset 기준값
                necroProfile.hpMul,
                perkState.AllyHpAdd,
                perkState.AllyHpMul
            );

            hp.SetMaxHp(maxHp);

            // === Attack ===
            float atkMul = NecroUndeadStatFormula.ComputeAttackMul(
                ctrl.def.attack.attackDamage,              // EnemyStatAsset 기준값
                necroProfile.attackMul,
                perkState.AllyDamageAdd,
                perkState.AllyDamageMul
            );

            ctrl.SetAttackMul(atkMul);
        }

        void FindNearestCorpse()
        {
            if (!player) return;

            Vector2 pos = player.transform.position;
            var hits = Physics2D.OverlapCircleAll(pos, findRange, corpseLayer);

            EnemyHp best = null;
            float bestDist = float.MaxValue;

            foreach (var h in hits)
            {
                var hp = h.GetComponentInParent<EnemyHp>();
                if (!hp) continue;

                var ctrl = hp.GetComponent<EnemyContext>();
                if (!ctrl || ctrl.Faction != Faction.Corpse) continue;

                float d = (hp.transform.position - player.transform.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = hp;
                }
            }

            SetHighlight(hoverCorpse, false);
            hoverCorpse = best;
            SetHighlight(hoverCorpse, true);
        }

        void SetHighlight(EnemyHp hp, bool on)
        {
            if (!hp) return;

            var anim = hp.GetComponent<Animator>();
            if (!anim) return;
        }

        // 2. 마우스 아래 언데드 시체폭발
        void KillUndeadUnderMouse()
        {
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var hit = Physics2D.Raycast(mouse, Vector2.zero, 0f, undeadLayer);
            if (!hit.collider) return;

            var hp = hit.collider.GetComponentInParent<EnemyHp>();
            var ctrl = hit.collider.GetComponentInParent<EnemyContext>();
            if (!hp || !ctrl) return;

            if (ctrl.Faction != Faction.Ally) return;

            Vector3 pos = hp.transform.position;

            // 폭발 파라미터(일단 하드코딩, 나중에 퍼크 런타임 값으로 교체)
            float radius = 2.5f;
            int damage = 1557;

            var gm = Necrogue.Game.Systems.GameManager.Instance;
            if (gm != null && gm.Pools != null)
                gm.Pools.UndeadExplosions?.Get(pos, radius, damage);

            hp.Damaged(999999);
        }


        // API

        public void NecromancerLevelUp()
        {
            necromancerLevel++;
        }
        public void NecromancerLevelDown()
        {
            necromancerLevel--;
        }
    }
}
