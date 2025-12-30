using UnityEngine;

[CreateAssetMenu(menuName = "Player/WeaponProfile")]
public class WeaponProfile : ScriptableObject
{
    [Header("총알 프리팹")]
    public PlayerBullet bulletPrefab;

    [Header("발사 주기(초)")]
    public float fireInterval = 1f;

    [Header("탄 속도")]
    public float bulletSpeed = 12f;

    [Header("탄 수명")]
    public float bulletLifeTime = 2f;

    [Header("풀 초기 크기")]
    public int poolInitialSize = 100;
}