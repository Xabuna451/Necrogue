using UnityEngine;
using Necrogue.Player.Runtime;

namespace Necrogue.Player.Runtime
{
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

            // Animator가 없으면 경고 (애니메이션 동작 안 함)
            if (!anim)
            {
                Debug.LogWarning("[PlayerAnimation] Animator 컴포넌트가 없습니다.");
            }
        }

        void Update()
        {
            if (!player || !player.InputManager || !anim) return;

            var input = player.InputManager;
            anim.SetBool("AnyKey", input.AnyKey);
            anim.SetFloat("LR", input.H, 0.1f, Time.deltaTime);
            anim.SetFloat("UD", input.V, 0.1f, Time.deltaTime);
        }
    }
}