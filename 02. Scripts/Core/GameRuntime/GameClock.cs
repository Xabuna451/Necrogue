using UnityEngine;

public class GameClock : MonoBehaviour
{
    float timer;
    public float Timer => timer;

    float elapsed;
    public float Elapsed => elapsed;

    void Update()
    {
        timer += Time.deltaTime;
        elapsed += Time.deltaTime;
    }

    public void SkipTime(float seconds)
    {
        elapsed += Mathf.Max(0f, seconds);
        timer += Mathf.Max(0f, seconds);
        // timer도 같이 올려두면 스킵 직후 스폰이 즉시 진행돼서 테스트가 빨라짐

        Debug.Log($"[EnemySpawner] Skip +{seconds}s => elapsed={elapsed:F1}");
    }

    public void ResetTimer()
    {
        timer = 0f;
    }
}
