using UnityEngine;

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

    public void SetAlly()
    {
        spriteRenderer.color = Color.cyan; // Ally 색상 유지
    }

    public void SetElite(EnemyEliteProfile elite)
    {
        transform.localScale = baseScale * elite.original.visual.EliteScaleMul;
        spriteRenderer.color = elite.original.visual.EliteColor;
    }
}