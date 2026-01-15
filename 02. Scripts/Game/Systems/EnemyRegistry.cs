using System.Collections.Generic;
using UnityEngine;

using Necrogue.Enemy.Runtime;

namespace Necrogue.Game.Systems
{
    public class EnemyRegistry : MonoBehaviour
    {
        public static EnemyRegistry Instance;

        readonly HashSet<EnemyContext> enemy = new();
        readonly HashSet<EnemyContext> ally = new();

        [SerializeField] Transform player;

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
            Debug.Log($"[EnemyRegistry] Player set: {(player ? player.name : "null")}");
        }

        public Transform GetPlayer()
        {
            return player;
        }

        public void Add(EnemyContext e)
        {
            if (!e) return;
            Remove(e);

            if (e.Faction == Faction.Enemy)
                enemy.Add(e);
            else if (e.Faction == Faction.Ally)
                ally.Add(e);
        }

        public void Remove(EnemyContext e)
        {
            if (!e) return;
            enemy.Remove(e);
            ally.Remove(e);
        }

        public HashSet<EnemyContext> GetOpposite(EnemyContext me)
        {
            return me.Faction == Faction.Enemy ? ally : enemy;
        }

        // == DEBUG INFO ==
        [SerializeField] List<EnemyContext> debugEnemy = new();
        [SerializeField] List<EnemyContext> debugAlly = new();

        void LateUpdate()
        {
            debugEnemy.Clear();
            debugAlly.Clear();

            debugEnemy.AddRange(enemy);
            debugAlly.AddRange(ally);
        }
    }
}