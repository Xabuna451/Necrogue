using UnityEngine;

using Necrogue.Game.CombatUI;

using Necrogue.Enemy.Runtime;
using Necrogue.Enemy.Data;

using Necrogue.Player.Runtime;

using Necrogue.Weapon.Runtime;


namespace Necrogue.RuntimeObject
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] EnemyPool enemyPool;
        [SerializeField] PlayerBulletPool playerBulletPool;
        [SerializeField] RewardPool rewardPool;
        [SerializeField] DamagePopupPool damagePopupPool;



        public EnemyPool Enemies => enemyPool;
        public PlayerBulletPool Bullets => playerBulletPool;
        public RewardPool Rewards => rewardPool;
        public DamagePopupPool DamagePopups => damagePopupPool;
        bool initialized;

        public void Init(
            EnemyDefAsset[] enemyDefs, int enemyEach,
            PlayerBullet bulletPrefab, int bulletSize,
            Exp expPrefab, Gold goldPrefab, int rewardEach
        )
        {
            if (initialized) return;

            enemyPool?.Init(enemyDefs, enemyEach);
            playerBulletPool?.Init(bulletPrefab, bulletSize);
            rewardPool?.Init(expPrefab, goldPrefab, rewardEach);
            damagePopupPool?.Init();

            initialized = true;
        }
    }
}
