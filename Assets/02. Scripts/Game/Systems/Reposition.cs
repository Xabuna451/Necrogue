using UnityEngine;
using Necrogue.Game.Systems;

namespace Necrogue.Game.Systems
{
    public class Reposition : MonoBehaviour
    {
        public float reposition;
        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag("Area")) return;

            Vector3 playerPos = GameManager.Instance.player.transform.position;
            Vector3 myPos = this.transform.position;
            Vector3 playerDir = GameManager.Instance.player.InputManager.Move;

            // float dirX = playerPos.x - myPos.x;
            // float dirY = playerPos.y - myPos.y;
            // float diffX = Mathf.Abs(dirX);
            // float diffY = Mathf.Abs(dirY);

            // dirX = dirX > 0 ? 1 : -1;
            // dirY = dirY > 0 ? 1 : -1;


            float diffX = Mathf.Abs(playerPos.x - myPos.x);
            float diffY = Mathf.Abs(playerPos.y - myPos.y);

            float dirX = playerDir.x > 0 ? 1 : -1;
            float dirY = playerDir.y > 0 ? 1 : -1;

            switch (transform.tag)
            {
                case "Ground":
                    if (diffX > diffY)
                    {
                        transform.Translate(Vector3.right * dirX * reposition);
                    }
                    else if (diffX < diffY)
                    {
                        transform.Translate(Vector3.up * dirY * reposition);
                    }

                    break;
            }
        }
    }
}