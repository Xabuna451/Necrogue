using UnityEngine;

namespace Necrogue.Perk.Data
{
    [CreateAssetMenu(menuName="Perk/RarityTable")]
    public class PerkRarityTableSO : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public PerkRarity rarity; // 기존 enum
            [Min(0f)] public float weight; // rarity 선택 가중치
        }

        public Entry[] entries = new Entry[]
        {
            new Entry{ rarity = PerkRarity.Common,    weight = 60f },
            new Entry{ rarity = PerkRarity.Rare,      weight = 25f },
            new Entry{ rarity = PerkRarity.Epic,      weight = 10f },
            new Entry{ rarity = PerkRarity.Legendary, weight = 4f  },
            new Entry{ rarity = PerkRarity.Fable,     weight = 1f  },
        };
    }
}
