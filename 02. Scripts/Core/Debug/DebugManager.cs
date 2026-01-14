using UnityEngine;

using Necrogue.Player.Runtime;

public class DebugManager : MonoBehaviour
{
    Player player;

    void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
            DebugCommands.SetPlayerHp(player, 1000);

        if (Input.GetKeyDown(KeyCode.F2))
            DebugCommands.SetPlayerAttack(player, 10000);

        if (Input.GetKeyDown(KeyCode.F3))
            DebugCommands.FastTime(2f);
        if (Input.GetKeyDown(KeyCode.F4))
            DebugCommands.SlowTime(2f);
#endif
    }
}

public static class DebugCommands
{
    public static void SetPlayerHp(Player player, int hp)
    {
        if (player == null) return;
        player.Hp.SetHpDirect(hp);
        Debug.Log($"[DEBUG] Player HP set to {hp}");
    }

    public static void SetPlayerAttack(Player player, int atk)
    {
        if (player == null) return;
        //player.Attack.SetAttackPowerDirect(atk);
        Debug.Log($"[DEBUG] Player ATK set to {atk}");
    }

    public static void FastTime(float seconds)
    {
        Time.timeScale += seconds;
        Debug.Log($"[DEBUG] Time scale boosted");
    }
    public static void SlowTime(float seconds)
    {
        Time.timeScale -= seconds;
        Debug.Log($"[DEBUG] Time scale slowed");
    }
}
