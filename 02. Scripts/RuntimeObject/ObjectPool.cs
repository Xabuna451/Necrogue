using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] EnemyPool enemyPool;
    [SerializeField] PlayerBulletPool playerBulletPool;
    [SerializeField] RewardPool rewardPool;

    public EnemyPool Enemies => enemyPool;
    public PlayerBulletPool Bullets => playerBulletPool;
    public RewardPool Rewards => rewardPool;

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

        initialized = true;
    }
}
