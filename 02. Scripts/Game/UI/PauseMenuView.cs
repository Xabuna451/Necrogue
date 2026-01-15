using UnityEngine;

using Necrogue.Game.Systems;

namespace Necrogue.Perk.UI
{
    public class PauseMenuView : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] GameObject perkPanel;     // PerkHistoryPanel이 들어있는 패널
        [SerializeField] GameObject optionsPanel;  // 옵션 패널(없으면 비워도 됨)

        [Header("Refs")]
        [SerializeField] PerkHistoryPanel perkHistory; // PerkHistoryPanel 스크립트

        [Header("Default")]
        [SerializeField] bool openPerkTabByDefault = true;

        void Awake()
        {
            // 안전: 옵션 패널 없으면 null이어도 OK
            if (!perkHistory)
                perkHistory = GetComponentInChildren<PerkHistoryPanel>(true);
        }

        void OnEnable()
        {
            // PauseMenuRoot가 켜질 때마다 초기 탭 세팅
            if (openPerkTabByDefault)
                ShowPerkTab();
            else
                ShowOptionsTab();

            // 열릴 때 최신 퍼크 갱신
            perkHistory?.Refresh();
        }

        // =========================
        // UI 버튼에서 호출할 함수들
        // =========================

        public void ShowPerkTab()
        {
            if (perkPanel) perkPanel.SetActive(true);
            if (optionsPanel) optionsPanel.SetActive(false);

            // 탭 전환 순간에도 갱신(퍼크가 변했을 수도)
            perkHistory?.Refresh();
        }

        public void ShowOptionsTab()
        {
            if (perkPanel) perkPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(true);
        }

        public void Close()
        {
            // View는 게임 상태를 직접 만지지 않고 GameManager에 요청
            if (GameManager.Instance != null)
                GameManager.Instance.SetPaused(false);
        }
    }
}