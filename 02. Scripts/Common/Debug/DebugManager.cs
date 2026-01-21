using UnityEngine;
using PlayerType = Necrogue.Player.Runtime.Player;

namespace Necrogue.Common.Debug
{
    public class DebugManager : MonoBehaviour
    {
        [SerializeField] private PlayerType player;

        void Awake()
        {
            if (!player)
                player = FindFirstObjectByType<PlayerType>();
        }

        void Update()
        {
            //#if UNITY_EDITOR
            if (!player) return;

            if (Input.GetKeyDown(KeyCode.F1))
                DebugCommands.SetPlayerHp(player, 1000);

            if (Input.GetKeyDown(KeyCode.F2))
                DebugCommands.SetPlayerAttack(player, 10000);

            if (Input.GetKeyDown(KeyCode.F3))
                DebugCommands.AddTimeScale(+2f);

            if (Input.GetKeyDown(KeyCode.F4))
                DebugCommands.AddTimeScale(-2f);

            if (Input.GetKeyDown(KeyCode.Y))
            {
                var clock = FindFirstObjectByType<GameClock>();
                clock.SetTimerZero();
            }
            //#endif
        }
    }

    public static class DebugCommands
    {
        public static void SetPlayerHp(PlayerType player, int hp)
        {
            if (!player) return;
            player.Hp.SetHpDirect(hp);
            UnityEngine.Debug.Log($"[DEBUG] Player HP set to {hp}");
        }

        public static void SetPlayerAttack(PlayerType player, int atk)
        {
            if (!player) return;
            //player.Attack.SetAttackPowerDirect(atk);
            UnityEngine.Debug.Log($"[DEBUG] Player ATK set to {atk}");
        }

        public static void AddTimeScale(float delta)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + delta, 0f, 10f);
            UnityEngine.Debug.Log($"[DEBUG] TimeScale = {Time.timeScale}");
        }
    }
}
