using UnityEngine;
using System.Collections.Generic;
using Necrogue.Perk.Runtime;
using Necrogue.Perk.Data;

namespace Necrogue.Perk.UI
{
    public class PerkSelectUI : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot;
        [SerializeField] private PerkCardUI cardPrefab;

        private readonly List<PerkCardUI> pool = new();
        private PerkSystem system;

        public void Open(PerkDef[] perks, PerkSystem system)
        {
            this.system = system;

            EnsurePool(perks.Length);

            for (int i = 0; i < pool.Count; i++)
                pool[i].gameObject.SetActive(false);

            for (int i = 0; i < perks.Length; i++)
            {
                var p = perks[i];
                if (p == null) continue;

                var card = pool[i];
                card.gameObject.SetActive(true);

                card.Bind(p, OnSelect);

                // 진화
                int curStack = (this.system != null) ? this.system.GetStack(p.perkId) : 0;
                bool willEvolve = (curStack + 1 >= p.maxStack) && !string.IsNullOrWhiteSpace(p.descriptionEvolution);
                card.SetEvolved(willEvolve);
            }

            gameObject.SetActive(true);
        }

        private void EnsurePool(int needed)
        {
            while (pool.Count < needed)
            {
                var card = Instantiate(cardPrefab, contentRoot);
                pool.Add(card);
            }
        }

        private void OnSelect(PerkDef perk)
        {
            system.AcquirePerk(perk);
            Close();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
