using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlashUI : MonoBehaviour
{
    [SerializeField] CanvasGroup group;   // DamageVignette에 붙은 CanvasGroup
    [SerializeField] float peakAlpha = 0.6f;
    [SerializeField] float inTime = 0.05f;
    [SerializeField] float outTime = 0.2f;

    Coroutine co;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    public void Play()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        // 빠르게 올라가고
        float t = 0f;
        while (t < inTime)
        {
            t += Time.unscaledDeltaTime; // 일시정지 영향 없이(원하면 Time.deltaTime로 바꿔)
            group.alpha = Mathf.Lerp(0f, peakAlpha, t / inTime);
            yield return null;
        }
        group.alpha = peakAlpha;

        // 천천히 내려가고
        t = 0f;
        while (t < outTime)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(peakAlpha, 0f, t / outTime);
            yield return null;
        }
        group.alpha = 0f;
        co = null;
    }
}
