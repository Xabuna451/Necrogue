using UnityEngine;
using TMPro;
using System;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] float lifeTime = 0.8f;
    [SerializeField] float riseSpeed = 1.2f; // 월드 유닛/초

    float timer;
    Color baseColor;

    public event Action<DamagePopup> OnFinished;

    void OnEnable()
    {
        timer = 0f;
    }

    public void Show(int damage, Vector3 worldPos, Color color)
    {
        if (!text) return;

        transform.position = worldPos;

        text.text = damage.ToString();

        if (color != null) baseColor = color;
        else baseColor = text.color;

        text.color = baseColor;

        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 위로 이동
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);

        // 페이드아웃
        float t = lifeTime <= 0f ? 1f : Mathf.Clamp01(timer / lifeTime);
        var c = baseColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        text.color = c;

        if (timer >= lifeTime)
            Finish();
    }

    void Finish()
    {
        OnFinished?.Invoke(this);
    }
}
