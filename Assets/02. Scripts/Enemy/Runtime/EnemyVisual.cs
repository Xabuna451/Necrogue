using UnityEngine;

using Necrogue.Enemy.Data;
namespace Necrogue.Enemy.Runtime
{
    public class EnemyVisual : MonoBehaviour
    {
        private EnemyContext ctx;
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer SR => spriteRenderer;

        private Vector3 baseScale;

        public void Init(EnemyContext ctx)
        {
            this.ctx = ctx;
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseScale = transform.localScale;
        }

        public void Reset()
        {
            transform.localScale = baseScale;
            spriteRenderer.color = ctx.def.visual.EnemyColor;
        }

        public void SetFaction(Faction f)
        {
            if (!spriteRenderer) return;

            // 최소 변경: 기존 정책 유지
            if (f == Faction.Enemy)
                spriteRenderer.color = ctx.def.visual.EnemyColor;
            else if (f == Faction.Ally)
                spriteRenderer.color = Color.cyan;
            else // Corpse
                spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
        }

        // 엘리트는 “크기/색만” 덮어씌우는 용도로 분리
        public void SetEliteLookOnly(EnemyEliteProfile elite)
        {

            transform.localScale = baseScale * elite.original.visual.EliteScaleMul;
            spriteRenderer.color = elite.original.visual.EliteColor;
            // ctx에 elite 런타임 데이터가 없으면
            // 최소 변경으로는 EnemyContext가 ApplyElite에서 Visual.SetElite(elite) 호출하니까
            // 여기선 '색'만 유지하고 싶다면 elite 색을 저장해두는 방식이 필요함.
            // -> 최소 변경으로는 그냥 비워둬도 됨(엘리트 상태에서 팩션 바뀌는 케이스가 적다면)
        }
        public void SetEliteLookOnly()
        {
            transform.localScale = baseScale * ctx.def.visual.EliteScaleMul;
            spriteRenderer.color = ctx.def.visual.EliteColor;
        }

        public Sprite GetSprite()
        {
            return ctx.def.visual.undeadSprite;
        }
    }
}