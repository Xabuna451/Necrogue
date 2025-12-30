using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] int amount;
    public int Amount => amount;

    public RewardPool OwnerPool { get; set; }

    [SerializeField] private LayerMask playerLayer;

    public void Init(int amount)
    {
        this.amount = Mathf.Max(0, amount);
    }

    public void ResetForSpawn()
    {
        // 필요하면 초기화
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;

        // 획득 처리
        OwnerPool?.Return(this);
    }
}
