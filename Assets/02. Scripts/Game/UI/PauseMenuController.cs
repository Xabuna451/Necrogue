using Necrogue.Common.Data;
using UnityEngine;

using Necrogue.Game.Systems;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] InputManager input;
    [SerializeField] GameObject root;

    GameManager gm;

    void Awake()
    {
        if (!input) input = FindFirstObjectByType<InputManager>();
        if (root) root.SetActive(false);
    }

    void OnEnable()
    {
        gm = GameManager.Instance;
        if (gm == null) gm = FindFirstObjectByType<GameManager>();

        if (gm == null)
        {
            Debug.LogError("[PauseMenuController] GameManager not found");
            return;
        }

        gm.OnGameStateChanged += HandleState;
    }

    void OnDisable()
    {
        if (gm != null) gm.OnGameStateChanged -= HandleState;
    }

    void Update()
    {
        if (input != null && input.ESC && gm != null) gm.SetPaused(!gm.IsPaused);
    }

    void HandleState(GameState prev, GameState next)
    {
        if (!root) return;
        root.SetActive(next == GameState.Pause);
    }
}
