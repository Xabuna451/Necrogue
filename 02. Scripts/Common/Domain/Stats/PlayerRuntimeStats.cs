

namespace Necrogue.Core.Domain.Stats
{
    [System.Serializable]
    public class PlayerRuntimeStats
    {
        public int maxHp;
        public int attack;
        public float speed;

        public NecromancerProfile necromancerProfile;

        public void SetFromBase(PlayerStatAsset baseStats)
        {
            maxHp = baseStats.baseMaxHp;
            attack = baseStats.baseAttack;
            speed = baseStats.baseSpeed;
            necromancerProfile = baseStats.necromaner;
        }
    }
}
