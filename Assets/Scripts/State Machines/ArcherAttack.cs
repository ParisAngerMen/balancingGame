using UnityEngine;

[System.Serializable]
public class ArcherAttack : EnemyAttackState
{
    public ArcherAttack(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void FrameUpdate()
    {
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
