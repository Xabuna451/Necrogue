using UnityEngine;
using UnityEngine.UI;
using System;
using Necrogue.Perk.Data;
using TMPro;

namespace Necrogue.Perk.UI
{
    public class PerkCardUI : MonoBehaviour
    {
        private bool evolved;

        [Header("UI")]
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descText;
        [SerializeField] private TMP_Text tierText;

        [Header("Tier Gradient Flow")]
        [SerializeField] private bool animateTierGradient = true;
        [SerializeField, Min(0f)] private float flowSpeed = 1.5f;   // 초당 흐름 속도
        [SerializeField, Range(0f, 1f)] private float bandWidth = 0.35f; // 경계 부드러움(띠 두께)
        [Header("Flow Tuning (Water-like)")]
        [SerializeField, Range(0f, 1f)] private float highlightWidth = 0.18f;     // 하이라이트 띠 두께
        [SerializeField, Range(0f, 2f)] private float highlightStrength = 0.45f;  // 하이라이트 밝기(강도)
        [SerializeField] private float waveAmplitude = 0.08f;                    // 물결 흔들림 크기(0~0.15 추천)
        [SerializeField] private float waveFrequency = 2.0f;                     // 물결 빈도(1~4 추천)

        private PerkDef perk;
        private Action<PerkDef> onClick;

        // 현재 희귀도 / 그라데이션 색(Top/Bottom)
        private PerkRarity currentRarity;
        private Color topColor = Color.white;
        private Color bottomColor = Color.gray;

        // 애니 상태
        private float flowT;
        private bool gradientDirty;

        public void Bind(PerkDef perk, Action<PerkDef> onClick)
        {
            this.perk = perk;
            this.onClick = onClick;

            if (perk == null) { Clear(); return; }

            if (icon) icon.sprite = perk.icon;
            if (nameText) nameText.text = perk.displayName;

            // 기본은 일반 설명
            evolved = false;
            ApplyDef();

            ApplyTier(perk.rarity);
        }

        void Clear()
        {
            if (icon) icon.sprite = null;
            if (nameText) nameText.text = "";
            if (descText) descText.text = "";

            if (tierText)
            {
                tierText.text = "";
                tierText.colorGradient = new VertexGradient(Color.white);
                tierText.ForceMeshUpdate();
            }

            perk = null;
            flowT = 0f;
            gradientDirty = false;
        }

        public void SetEvolved(bool isEvolved)
        {
            evolved = isEvolved;
            ApplyDef();
        }

        void ApplyDef()
        {
            if (!descText) return;
            if (!nameText) return;
            if (perk == null) { descText.text = ""; return; }
            if (perk.description == null) { descText.text = ""; return; }


            if (evolved && !string.IsNullOrWhiteSpace(perk.descriptionEvolution))
            {
                descText.text = perk.descriptionEvolution;
                nameText.text = perk.displayNameEvolution;
            }
            else
            {
                descText.text = perk.description;
                nameText.text = perk.displayName;
            }
        }

        void ApplyTier(PerkRarity rarity)
        {
            if (!tierText) return;

            currentRarity = rarity;
            tierText.text = rarity.ToString();

            // 희귀도별 Top/Bottom 대표색 결정
            // (좌우는 동일하게 두고, 위/아래만 대비 줌)
            switch (rarity)
            {
                case PerkRarity.Common:
                    topColor = Color.white;
                    bottomColor = Color.gray;
                    break;

                case PerkRarity.Rare:
                    topColor = new Color(0.1f, 0.4f, 1f);
                    bottomColor = new Color(0.4f, 0.9f, 1f);
                    break;

                case PerkRarity.Epic:
                    topColor = new Color(0.5f, 0.2f, 1f);
                    bottomColor = new Color(1f, 0.6f, 1f);
                    break;

                case PerkRarity.Legendary:
                    topColor = new Color(1f, 0.5f, 0.1f);
                    bottomColor = new Color(1f, 0.9f, 0.5f);
                    break;

                case PerkRarity.Fable:
                    topColor = Color.white;
                    bottomColor = Color.red;
                    break;

                default:
                    topColor = Color.white;
                    bottomColor = Color.white;
                    break;
            }

            // 기본(정적) 그라데이션 한 번 세팅
            tierText.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);

            // 애니 재시작
            flowT = 0f;
            gradientDirty = true;

            // 버텍스 접근을 위해 메쉬 갱신
            tierText.ForceMeshUpdate();
        }

        void Update()
        {
            if (!animateTierGradient) return;
            if (!tierText) return;
            if (string.IsNullOrEmpty(tierText.text)) return;

            // 텍스트가 바뀐 직후 1프레임은 강제 갱신해주는 편이 안전
            if (gradientDirty)
            {
                tierText.ForceMeshUpdate();
                gradientDirty = false;
            }

            flowT += Time.unscaledDeltaTime * flowSpeed;

            ApplyFlowingVerticalGradient(tierText, topColor, bottomColor, flowT, highlightWidth, highlightStrength, waveAmplitude, waveFrequency);
        }

        /// <summary>
        /// TMP 메쉬 버텍스 컬러를 매 프레임 갱신해서
        /// "위->아래로 흐르는" 느낌을 만든다.
        ///
        /// 원리:
        /// - 각 버텍스의 Y 위치를 0~1로 정규화
        /// - (y - time) 를 이용해 경계가 이동하는 값을 만들고
        /// - SmoothStep으로 부드러운 전이(띠)를 준다
        /// </summary>
        static void ApplyFlowingVerticalGradient(
            TMP_Text text,
            Color top,
            Color bottom,
            float time,
            float highlightWidth01,
            float highlightStrength,
            float waveAmp01,
            float waveFreq
        )
        {
            // 텍스트 메쉬 최신화
            text.ForceMeshUpdate();

            var ti = text.textInfo;
            int charCount = ti.characterCount;
            if (charCount == 0) return;

            // 전체 텍스트 Y 범위
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            for (int i = 0; i < charCount; i++)
            {
                var c = ti.characterInfo[i];
                if (!c.isVisible) continue;

                int m = c.materialReferenceIndex;
                int v = c.vertexIndex;
                var verts = ti.meshInfo[m].vertices;

                minY = Mathf.Min(minY, verts[v + 0].y, verts[v + 1].y, verts[v + 2].y, verts[v + 3].y);
                maxY = Mathf.Max(maxY, verts[v + 0].y, verts[v + 1].y, verts[v + 2].y, verts[v + 3].y);
            }

            float range = Mathf.Max(0.0001f, maxY - minY);

            // 하이라이트가 내려가는 중심(0~1)
            // 아래로 흐르는 느낌: time이 증가하면 center가 감소하도록
            float center = Mathf.Repeat(1f - (time * 0.35f), 1f);

            // 튜닝 값 클램프
            float w = Mathf.Clamp(highlightWidth01, 0.01f, 0.6f);
            float hs = Mathf.Clamp(highlightStrength, 0f, 2f);
            float amp = Mathf.Clamp(waveAmp01, 0f, 0.3f);
            float freq = Mathf.Max(0.01f, waveFreq);

            for (int i = 0; i < charCount; i++)
            {
                var c = ti.characterInfo[i];
                if (!c.isVisible) continue;

                int m = c.materialReferenceIndex;
                int v = c.vertexIndex;

                var colors = ti.meshInfo[m].colors32;
                var verts = ti.meshInfo[m].vertices;

                for (int k = 0; k < 4; k++)
                {
                    // 0(아래)~1(위)
                    float y01 = (verts[v + k].y - minY) / range;

                    // ---- (1) 기본 그라데이션: 아래→위
                    float baseT = y01;
                    Color baseCol = Color.Lerp(bottom, top, baseT);

                    // ---- (2) 물결: 하이라이트 위치를 y에 따라 살짝 흔들기
                    // y에 따라 x축 위치가 없어도 "흐름"처럼 느껴지게 하는 편법:
                    // y01에 사인을 섞어서 center 비교값을 굴절시킴
                    float ripple = Mathf.Sin((y01 * freq + time) * Mathf.PI * 2f) * amp;

                    // 하이라이트 띠까지의 거리(원형 반복)
                    // center가 순환하므로, 가장 가까운 거리로 계산
                    float dy = Mathf.Abs((y01 + ripple) - center);
                    dy = Mathf.Min(dy, 1f - dy); // wrap 거리

                    // ---- (3) 하이라이트 마스크: 띠 중심에 가까울수록 1
                    // dy=0 -> 1, dy>=w -> 0 (부드럽게)
                    float mask = 1f - Mathf.SmoothStep(0f, w, dy);

                    // ---- (4) 하이라이트 컬러: baseCol을 밝게(윤기)
                    // 그냥 흰색을 섞는 게 가장 자연스러움
                    Color hiCol = Color.Lerp(baseCol, Color.white, 0.65f);

                    // 최종: base + highlight
                    Color final = Color.Lerp(baseCol, hiCol, mask * hs);

                    colors[v + k] = final;
                }
            }

            // 메쉬 반영
            for (int m = 0; m < ti.meshInfo.Length; m++)
            {
                var mi = ti.meshInfo[m];
                mi.mesh.colors32 = mi.colors32;
                text.UpdateGeometry(mi.mesh, m);
            }
        }


        // 0~1 입력을 부드러운 그라데이션으로 만들기 (띠 전이)
        static float SmoothBand(float x01, float width01)
        {
            // width01이 작을수록 경계가 날카로움
            float w = Mathf.Clamp(width01, 0.001f, 0.999f);

            // 0..1 전체를 부드럽게: 아래(0)→위(1)
            // 가운데 영역을 더 길게 쓰고 싶으면 여기 커브를 바꾸면 됨.
            // 지금은 기본 SmoothStep 2번으로 충분히 "흐르는" 느낌 남.
            float a = Mathf.SmoothStep(0f, w, x01);
            float b = Mathf.SmoothStep(1f - w, 1f, x01);
            return Mathf.Clamp01(a * (1f - b) + b);
        }

        public void OnClick()
        {
            if (perk == null) return;
            onClick?.Invoke(perk);
        }
    }
}
