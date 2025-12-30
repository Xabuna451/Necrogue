using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    public static EnemyRegistry Instance { get; private set; }

    readonly HashSet<EnemyCtrl> enemy = new();
    readonly HashSet<EnemyCtrl> ally = new();

    Transform player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetPlayer(Transform t)
    {
        player = t;
    }

    public Transform GetPlayer()
    {
        return player;
    }

    public void Add(EnemyCtrl e)
    {
        if (!e) return;
        Remove(e);

        if (e.Faction == Faction.Enemy) enemy.Add(e);
        else if (e.Faction == Faction.Ally) ally.Add(e);
    }

    public void Remove(EnemyCtrl e)
    {
        if (!e) return;
        enemy.Remove(e);
        ally.Remove(e);
    }

    public HashSet<EnemyCtrl> GetOpposite(EnemyCtrl me)
    {
        return me.Faction == Faction.Enemy ? ally : enemy;
    }






    // == DEBUG INFO ==
    [SerializeField] List<EnemyCtrl> debugEnemy = new();
    [SerializeField] List<EnemyCtrl> debugAlly = new();

    void LateUpdate()
    {
        debugEnemy.Clear();
        debugAlly.Clear();

        debugEnemy.AddRange(enemy);
        debugAlly.AddRange(ally);
    }
}
