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
                // 씬에 PerkSystem이 하나뿐이라서 찾아서 연결
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