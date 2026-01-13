using UnityEngine;
using UnityEngine.UI;

public class GameClockUI : MonoBehaviour
{
    [SerializeField] GameClock gameClock;
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        if (!gameClock)
            gameClock = FindFirstObjectByType<GameClock>();
    }

    void Update()
    {
        if (!gameClock || !text) return;

        float elapsed = gameClock.Elapsed;
        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);
        text.text = $"{minutes:00}:{seconds:00}";
    }

}
