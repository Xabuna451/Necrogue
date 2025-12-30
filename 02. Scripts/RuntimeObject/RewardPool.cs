using System.Collections.Generic;
using UnityEngine;

public class RewardPool : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Exp expPrefab;
    [SerializeField] private Gold goldPrefab;

    readonly Queue<Exp> expPool = new();
    readonly Queue<Gold> goldPool = new();

    bool initialized;

    public void Init(Exp expPrefab, Gold goldPrefab, int initialEach)
    {
        if (initialized) return;

        if (!expPrefab || !goldPrefab)
        {
            Debug.LogError("[RewardPool] prefab null");
            return;
        }

        this.expPrefab = expPrefab;
        this.goldPrefab = goldPrefab;

        initialEach = Mathf.Max(1, initialEach);

        for (int i = 0; i < initialEach; i++)
        {
            CreateNewExp();
            CreateNewGold();
        }

        initialized = true;
    }

    Exp CreateNewExp()
    {
        var e = Instantiate(expPrefab, transform);
        e.gameObject.SetActive(false);
        e.OwnerPool = this;
        expPool.Enqueue(e);
        return e;
    }

    Gold CreateNewGold()
    {
        var g = Instantiate(goldPrefab, transform);
        g.gameObject.SetActive(false);
        g.OwnerPool = this;
        goldPool.Enqueue(g);
        return g;
    }

    public Exp GetExp()
    {
        if (!initialized) { Debug.LogError("[RewardPool] Init 먼저"); return null; }

        if (expPool.Count == 0) CreateNewExp();
        var e = expPool.Dequeue();
        e.gameObject.SetActive(true);
        e.ResetForSpawn();
        return e;
    }

    public Gold GetGold()
    {
        if (!initialized) { Debug.LogError("[RewardPool] Init 먼저"); return null; }

        if (goldPool.Count == 0) CreateNewGold();
        var g = goldPool.Dequeue();
        g.gameObject.SetActive(true);
        g.ResetForSpawn();
        return g;
    }

    public void Return(Exp e)
    {
        if (!e) return;
        e.gameObject.SetActive(false);
        expPool.Enqueue(e);
    }

    public void Return(Gold g)
    {
        if (!g) return;
        g.gameObject.SetActive(false);
        goldPool.Enqueue(g);
    }
}
