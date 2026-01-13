using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class SquareVignetteTex : MonoBehaviour
{
    public int width = 1920;
    public int height = 1050;

    [Range(0.0f, 0.95f)] public float inner = 0.74f;
    [Range(0.01f, 0.6f)] public float feather = 0.20f;
    [Range(0.5f, 3.0f)] public float exp = 2.1f;

    void Awake()
    {
        var raw = GetComponent<RawImage>();
        raw.texture = Create(width, height, inner, feather, exp);

        // 색은 RawImage.color(인스펙터)로 빨강 틴트
        raw.color = Color.red;
    }

    Texture2D Create(int w, int h, float inner, float feather, float exp)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float u = (x + 0.5f) / w;
                float v = (y + 0.5f) / h;

                float nx = (u - 0.5f) * 2f;
                float ny = (v - 0.5f) * 2f;

                // 모서리 포함 사각 테두리
                float d = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny)); // 0~1

                float a = Mathf.InverseLerp(inner, inner + feather, d);
                a = Mathf.Clamp01(a);
                a = Mathf.Pow(a, exp); // 바깥 더 진하게

                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }

        tex.Apply();
        return tex;
    }
}
