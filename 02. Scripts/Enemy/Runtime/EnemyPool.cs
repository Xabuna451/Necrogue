using System;
using System.Collections.Generic;
using UnityEngine;

using Necrogue.Enemy.Data;

namespace Necrogue.Enemy.Runtime
{
    public class EnemyPool : MonoBehaviour
    {
        [Header("런타임 초기화됨")]
        public EnemyDefAsset[] defs;
        public int initialEach = 20;

        readonly Dictionary<EnemyDefAsset, Queue<EnemyContext>> pools = new();
        bool initialized;

        public void Init(EnemyDefAsset[] enemyDefs, int sizeEach)
        {
            if (initialized) return;

            defs = enemyDefs;
            initialEach = sizeEach;

            if (defs == null || defs.Length == 0)
            {
                Debug.LogError("[EnemyPool] defs 비어 있음");
                return;
            }

            for (int i = 0; i < defs.Length; i++)
            {
                var def = defs[i];
                if (!def)
                {
                    Debug.LogError($"[EnemyPool] defs[{i}] null");
                    continue;
                }

                EnsurePool(def);

                for (int j = 0; j < initialEach; j++)
                    CreateNew(def);
            }

            initialized = true;
        }

        void EnsurePool(EnemyDefAsset def)
        {
            if (!pools.ContainsKey(def))
                pools.Add(def, new Queue<EnemyContext>());
        }

        EnemyContext CreateNew(EnemyDefAsset def)
        {
            if (!def || !def.enemyPrefab)
            {
                Debug.LogError("[EnemyPool] def 또는 def.prefab null");
                return null;
            }

            var go = Instantiate(def.enemyPrefab, transform);
            go.gameObject.SetActive(false);

            var e = go.GetComponent<EnemyContext>();
            if (!e)
            {
                Debug.LogError("[EnemyPool] def.prefab에 EnemyCtrl이 없음");
                Destroy(go);
                return null;
            }

            e.OwnerPool = this;
            e.OriginDef = def;
            e.def = def;

            pools[def].Enqueue(e);
            return e;
        }

        public EnemyContext GetEnemy(EnemyDefAsset def)
        {
            if (!initialized)
            {
                Debug.LogError("[EnemyPool] Init을 먼저 호출해야 함");
                return null;
            }
            if (!def)
            {
                Debug.LogError("[EnemyPool] def null");
                return null;
            }

            EnsurePool(def);

            var q = pools[def];
            if (q.Count == 0)
                CreateNew(def);

            if (q.Count == 0) return null; // CreateNew 실패 대비

            var e = q.Dequeue();
            e.gameObject.SetActive(true);

            e.OriginDef = def;
            e.def = def;

            e.Reset();
            return e;
        }

        public void Return(EnemyContext e)
        {
            if (!e) return;

            e.gameObject.SetActive(false);

            e.Reset();
            e.ChangeFaction(Faction.Enemy);

            var def = e.OriginDef;
            if (!def)
            {
                Debug.LogError("[EnemyPool] OriginDef 미설정: Return 불가");
                return;
            }

            EnsurePool(def);
            pools[def].Enqueue(e);
        }
    }
}