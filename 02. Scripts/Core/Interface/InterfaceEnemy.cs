using UnityEngine;

public interface IEnemyAttack
{
    void Init(EnemyContext ctx);
    void Tick();
}

public interface IEnemyMove
{
    void Init(EnemyContext ctx);
    void MoveTo(Vector2 target);
    void Stop();
}

public interface IEnemyAI
{
    void Init(EnemyContext ctx);
    void Tick();
}