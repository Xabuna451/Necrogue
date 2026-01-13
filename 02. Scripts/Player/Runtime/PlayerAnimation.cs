using UnityEngine;

using Necrogue.Player.Runtime;

public class PlayerAnimation : MonoBehaviour
{
    private Player player;
    private Animator anim;

    public void Init(Player player)
    {
        this.player = player;
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        PlayerMoveAnimation(player.InputManager.AnyKey, player.InputManager.H, player.InputManager.V);
    }

    void PlayerMoveAnimation(bool AnyKey, float LR, float UD)
    {
        if (!anim) return;
        anim.SetBool("AnyKey", AnyKey);
        anim.SetFloat("LR", LR, 0.1f, Time.deltaTime);
        anim.SetFloat("UD", UD, 0.1f, Time.deltaTime);
    }

}
