using UnityEngine;
using Necrogue.Perk.Data.Perk;

namespace Necrogue.Perk.Data
{
    public enum PerkCategory
    {
        Attack,
        Survival,
        Utility,
        Necro,
    }

    public enum PerkRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Fable,
    }

    [CreateAssetMenu(menuName = "Perk/PerkDef")]
    public class PerkDef : ScriptableObject
    {
        [Header("기본 정보")]
        public string perkId;              // 내부 ID (중복 금지, 코드용)
        public string displayName;         // UI 표시 이름

        [TextArea]
        public string description;         // UI 설명

        public Sprite icon;

        [Header("진화 여부 / 정보")]
        public bool canEvolve = false;
        public string displayNameEvolution;
        [TextArea]
        public string descriptionEvolution; // 진화버전 설명
        public Sprite iconEvolution;

        [Header("분류")]
        public PerkCategory category;      // Attack / Survival / Utility / Necro
        public PerkRarity rarity;          // Common / Rare / Epic / Legendary

        [Tooltip("등장 가중치 (높을수록 잘 나옴)")]
        [Min(0f)]
        public float weight = 1f;

        [Header("스택 / 효과")]
        [Min(1)]
        public int maxStack = 1;           // 기본은 1회성

        public PerkEffect[] effects;       // 이 특전이 적용하는 효과들
    }
}
