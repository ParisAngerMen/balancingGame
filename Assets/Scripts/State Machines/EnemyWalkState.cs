using UnityEngine;

[System.Serializable]
public class EnemyWalkState : EnemyState
{
    public EnemyWalkState(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        // Change animations

        // The base may already be destroyed (or missing) when the unit starts walking
        if (enemy.enemyBase != null)
        {
            enemy.MoveTo(enemy.enemyBase.transform.position);
        }
    }
}
