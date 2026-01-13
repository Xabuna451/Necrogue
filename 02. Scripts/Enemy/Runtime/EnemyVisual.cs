using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private EnemyCtrl ctx;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer SR => spriteRenderer;

    private Vector3 baseScale;

    private bool isAlly;
    [SerializeField] Color allyColor = Color.cyan;
    [SerializeField] Color enemyColor = Color.white;


    private bool eliteOn;
    [SerializeField] Color eliteColor = Color.red;
    [SerializeField] float eliteScaleMul = 1f;

    private bool corpseOn;
    [SerializeField] Color corpseColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);

    private bool resurrectOn;
    [SerializeField] Color resurrectColor = Color.cyan;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
    }

    public void Init(EnemyCtrl ctx)
    {
        this.ctx = ctx;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ====== 외부 API ======

    public void SetFaction(Faction faction)
    {
        if (faction == Faction.Enemy)
        {
            isAlly = false;
            Apply();
        }
        else if (faction == Faction.Ally)
        {
            isAlly = true;
            Apply();
        }
        else if (faction == Faction.Corpse)
        {
            isAlly = false;
            corpseOn = true;
            SetCorpse(corpseOn);
        }

    }

    public void SetElite(Color color, float scaleMul)
    {
        eliteOn = true;
        eliteColor = color;
        eliteScaleMul = Mathf.Max(0.1f, scaleMul);
        Apply();
    }

    public void ClearElite()
    {
        eliteOn = false;
        eliteScaleMul = 1f;
        Apply();
    }

    public void SetCorpse(bool on)
    {
        corpseOn = on;
        Apply();
    }

    public void SetResurrect(bool on)
    {
        resurrectOn = on;
        Apply();
    }

    // 풀링 Reset에서 호출 (단 하나의 리셋 함수만 둬라)
    public void ResetAll()
    {
        eliteOn = false;
        eliteColor = Color.white;
        eliteScaleMul = 1f;

        corpseOn = false;
        resurrectOn = false;

        Apply();
    }

    private void Apply()
    {
        if (!spriteRenderer) return;

        // 색 우선순위: Ally > Corpse > Resurrect > Elite > Default
        if (isAlly) spriteRenderer.color = allyColor;
        else if (!isAlly) spriteRenderer.color = enemyColor;
        else if (corpseOn) spriteRenderer.color = corpseColor;
        else if (resurrectOn) spriteRenderer.color = resurrectColor;
        else if (eliteOn) spriteRenderer.color = eliteColor;
        else spriteRenderer.color = Color.white;

        // 스케일: Elite만 적용(원하면 corpse도 가능)
        transform.localScale = baseScale * (eliteOn ? eliteScaleMul : 1f);
    }
}