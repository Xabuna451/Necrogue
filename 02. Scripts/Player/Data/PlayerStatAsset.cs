using UnityEngine;

[CreateAssetMenu(menuName = "Combat/PlayerHeartStats")]
public class PlayerStatAsset : ScriptableObject
{
    public int baseMaxHp = 200;
    public int baseAttack = 10;
    public float baseSpeed = 3f;

    public NecromancerProfile necromaner;
}