using UnityEngine;

using Necrogue.Player.Runtime;
using Necrogue.Core.Domain.Stats;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    [Header("이동 설정")]
    public float speed = 2f;

    Rigidbody2D rb;
    ContactFilter2D filter;
    RaycastHit2D[] hits = new RaycastHit2D[32];

    public Vector2 Position => rb ? rb.position : (Vector2)transform.position;

    public void Init(Player player)
    {
        this.player = player;
    }

    public void InitFromBase(PlayerStatAsset stats)
    {
        player.Stats = stats;
        speed = stats ? stats.baseSpeed : speed;
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 벽 충돌용 필터
        filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(LayerMask.GetMask("Wall"));
    }

    public void PhysicsMove()
    {

        if (rb == null) return;

        Vector2 moveDir = player.InputManager.Move;
        float moveDist = Time.fixedDeltaTime * speed;

        int hitCount = rb.Cast(moveDir, filter, hits, moveDist);
        if (hitCount == 0)
        {

            rb.MovePosition(rb.position + moveDir * moveDist);
        }
        else
        {
            Vector2 n = hits[0].normal;
            Vector2 slide = moveDir - Vector2.Dot(moveDir, n) * n;
            rb.MovePosition(rb.position + slide.normalized * moveDist);
        }
    }

    public void ApplyStats(PlayerRuntimeStats runtimeStats)
    {
        speed = runtimeStats.speed;
    }
}
