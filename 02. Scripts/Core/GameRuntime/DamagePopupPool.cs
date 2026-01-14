using System.Collections.Generic;
using UnityEngine;

public class DamagePopupPool : MonoBehaviour
{
    [SerializeField] DamagePopup prefab;
    [SerializeField] int size = 30;

    readonly Queue<DamagePopup> q = new();
    bool initialized;

    public void Init()
    {
        if (initialized) return;
        initialized = true;

        for (int i = 0; i < size; i++)
        {
            var p = Instantiate(prefab, transform);
            p.gameObject.SetActive(false);
            p.OnFinished += Return;
            q.Enqueue(p);
        }
    }

    public DamagePopup Get(Vector3 worldPos, int damage, Color color)
    {
        if (!initialized) Init();

        var p = q.Count > 0 ? q.Dequeue() : CreateOne();

        p.transform.SetParent(transform, false); // Canvas 아래 유지
        p.gameObject.SetActive(true);
        p.Show(damage, worldPos, color);

        return p;
    }

    DamagePopup CreateOne()
    {
        var p = Instantiate(prefab, transform);
        p.gameObject.SetActive(false);
        p.OnFinished += Return;
        return p;
    }

    void Return(DamagePopup p)
    {
        p.gameObject.SetActive(false);
        q.Enqueue(p);
    }
}
