namespace Necrogue.Common.Interfaces
{

    public interface IDamageSource
    {
        int Damage { get; }
        bool ConsumeOnHit { get; }   // 맞으면 제거할지(총알=true, 빔=false)
        void Despawn();              // 제거 처리 (풀 반환/비활성 등)
    }
}