using UnityEngine;

public class PlayerNecroController : MonoBehaviour
{
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
    }

    void Update()
    {
        FindNearestCorpse();

        if (input.RightClick)
            KillUndeadUnderMouse();
    }

    // 1. 가장 가까운 시체 찾고 하이라이트
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

            var ctrl = hp.GetComponent<EnemyCtrl>();
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

        var smb = anim.GetBehaviour<CorpseStateSMB>();
        smb?.SetHighlight(on);
    }

    // 2. 마우스 아래 언데드 즉사
    void KillUndeadUnderMouse()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var hit = Physics2D.Raycast(mouse, Vector2.zero, 0f, undeadLayer);
        if (!hit.collider) return;

        var hp = hit.collider.GetComponentInParent<EnemyHp>();
        var ctrl = hit.collider.GetComponentInParent<EnemyCtrl>();
        if (!hp || !ctrl) return;

        if (ctrl.Faction != Faction.Ally) return;

        hp.Damaged(999999);
    }
}
