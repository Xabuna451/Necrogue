using System.Collections.Generic;
using UnityEngine;
using Necrogue.Perk.Runtime;

namespace Necrogue.Perk.UI
{
    public class PerkHistoryPanel : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] PerkSystem perkSystem;
        [SerializeField] Transform content;
        [SerializeField] PerkIconSlotUI slotPrefab;

        // 슬롯 재사용 풀
        readonly List<PerkIconSlotUI> slots = new();

        void Awake()
        {
            if (!perkSystem)
            {
                // Player에서 perkSystem 들고 있으면 그쪽으로 붙여도 됨
                // 우선 런타임에서 찾기(포폴용으론 SerializeField 할당 추천)
                perkSystem = FindFirstObjectByType<PerkSystem>();
            }
        }

        void OnEnable()
        {
            if (perkSystem != null)
                perkSystem.OnPerksChanged += Refresh;

            Refresh();
        }

        void OnDisable()
        {
            if (perkSystem != null)
                perkSystem.OnPerksChanged -= Refresh;
        }

        public void Refresh()
        {
            if (perkSystem == null || content == null || slotPrefab == null) return;

            int i = 0;
            foreach (var (def, stack) in perkSystem.EnumerateOwnedPerksByAcquireOrder())
            {
                if (def == null) continue;

                var slot = GetOrCreate(i);
                slot.gameObject.SetActive(true);
                slot.Bind(def, stack);

                i++;
            }

            // 남는 슬롯 비활성
            for (int k = i; k < slots.Count; k++)
                slots[k].gameObject.SetActive(false);
        }

        PerkIconSlotUI GetOrCreate(int index)
        {
            while (slots.Count <= index)
            {
                var s = Instantiate(slotPrefab, content);
                slots.Add(s);
            }
            return slots[index];
        }
    }
}