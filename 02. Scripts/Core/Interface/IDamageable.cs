public interface IDamageable
{
    void Damaged(int damage);
    bool IsDead { get; }  // 선택사항, 필요 시 사용
}