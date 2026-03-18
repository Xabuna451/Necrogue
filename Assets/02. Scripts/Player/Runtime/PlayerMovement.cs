using UnityEngine;
using Necrogue.Core.Domain.Stats;
using Necrogue.Player.Runtime;

using Necrogue.Core.Domain.Necro;

public class PlayerMovement : MonoBehaviour, IStatAppliable
{
    // --------------------------------------------------
    // Fields & Properties
    // --------------------------------------------------
    private Player player;

    [Header("이동 설정")]
    [Tooltip("기본 이동 속도 (퍼크/스탯 미적용 시 값)")]
    [SerializeField] private float baseSpeed = 5f;

    private float currentSpeed;  // 실제로 사용하는 속도 (ApplyStats에서 갱신)

    private Rigidbody2D rb;
    private ContactFilter2D wallFilter;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[32];

    public Vector2 Position => rb ? rb.position : (Vector2)transform.position;


    // --------------------------------------------------
    // Init
    // --------------------------------------------------
    public void Init(Player player)
    {
        this.player = player;
    }

    // InitFromBase는 더 이상 필요 없음 (ApplyStats로 통합)
    // 만약 초기값 설정이 반드시 필요하다면 Awake에서 baseSpeed를 사용


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 벽 충돌용 필터 (한 번만 설정)
        wallFilter = new ContactFilter2D
        {
            useTriggers = false,
            layerMask = LayerMask.GetMask("Wall")
        };

        // 초기 속도 설정
        currentSpeed = baseSpeed;
    }


    // --------------------------------------------------
    // ApplyStats 구현
    // --------------------------------------------------
    public void ApplyStats(PlayerRuntimeStats playerStats, NecroRuntimeParams necroParams = null)
    {
        // Movement는 speed만 관심 있음
        currentSpeed = playerStats.speed;

        // 디버그용 (필요 시)
        // Debug.Log($"[PlayerMovement] Speed updated to {currentSpeed}");
    }


    // --------------------------------------------------
    // 물리 이동 (FixedUpdate에서 호출됨)
    // --------------------------------------------------
    public void PhysicsMove()
    {
        if (rb == null) return;

        Vector2 moveDir = player?.InputManager?.Move ?? Vector2.zero;

        // 이동하려는 거리
        float moveDist = Time.fixedDeltaTime * currentSpeed;

        // 이동 방향으로 Cast해서 벽 충돌 체크
        int hitCount = rb.Cast(moveDir, wallFilter, hits, moveDist);

        if (hitCount == 0)
        {
            // 충돌 없음 -> 그대로 이동
            rb.MovePosition(rb.position + moveDir * moveDist);
        }
        else
        {
            // 벽에 닿았을 때 슬라이딩
            Vector2 normal = hits[0].normal;
            Vector2 slide = moveDir - Vector2.Dot(moveDir, normal) * normal;
            rb.MovePosition(rb.position + slide.normalized * moveDist);
        }
    }
}