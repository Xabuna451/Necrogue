using UnityEngine;

[CreateAssetMenu(menuName = "Combat/PlayerHeartStats")]
public class PlayerStatAsset : ScriptableObject
{
    public int baseMaxHp = 20;
    public int baseAttack = 5;
    public float baseSpeed = 5f;

    public NecromancerProfile necromaner;
}
