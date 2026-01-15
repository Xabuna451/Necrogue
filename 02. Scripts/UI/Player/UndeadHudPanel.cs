using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Necrogue.Player.Runtime;
using Necrogue.Enemy.Data;

namespace Necrogue.UI.Player
{
    public class UndeadHudPanel : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private NecromancerController necro;
        [SerializeField] private TMP_Text slotText;

        [Header("List")]
        [SerializeField] private Transform content;
        [SerializeField] private UndeadSlotUI slotPrefab;

        readonly List<UndeadSlotUI> slots = new();

        struct SlotData
        {
            public int count;
            public Sprite icon; // 첫 발견 sprite로 고정
        }

        readonly Dictionary<EnemyDefAsset, SlotData> map = new();

        void Awake()
        {
            if (!necro) necro = FindFirstObjectByType<NecromancerController>();
        }

        void OnEnable()
        {
            if (necro != null)
                necro.OnUndeadChanged += Refresh;

            Refresh();
        }

        void OnDisable()
        {
            if (necro != null)
                necro.OnUndeadChanged -= Refresh;
        }

        public void Refresh()
        {
            if (!necro || !content || !slotPrefab) return;

            if (slotText)
                slotText.text = $"{necro.SlotUsed} / {necro.SlotMax}";

            map.Clear();

            var list = necro.UndeadList;


            for (int i = 0; i < list.Count; i++)
            {
                var e = list[i];
                if (!e || !e.def) continue;

                var def = e.def;

                Sprite icon = e.Visual != null
                    ? e.Visual.GetSprite()
                    : null;

                if (map.TryGetValue(def, out var data))
                {
                    data.count++;

                    // 최초 등록 시에만 아이콘 세팅
                    if (!data.icon && icon)
                        data.icon = icon;

                    map[def] = data;
                }
                else
                {
                    map.Add(def, new SlotData
                    {
                        count = 1,
                        icon = icon
                    });
                }
            }


            int idx = 0;
            foreach (var kv in map)
            {
                var data = kv.Value;

                var slot = GetOrCreate(idx);
                slot.gameObject.SetActive(true);
                slot.Bind(data.icon, data.count);

                idx++;
            }

            for (int i = idx; i < slots.Count; i++)
                slots[i].gameObject.SetActive(false);
        }

        UndeadSlotUI GetOrCreate(int index)
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
