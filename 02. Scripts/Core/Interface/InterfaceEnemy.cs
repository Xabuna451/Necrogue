using UnityEngine;

public interface IEnemyAttack
{
    void Init(EnemyCtrl ctx);
    void Tick();
}

public interface IEnemyMove
{
    void Init(EnemyCtrl ctx);
    void MoveTo(Vector2 target);
    void Stop();
}

public interface IEnemyAI
{
    void Init(EnemyCtrl ctx);
    void Tick();
}