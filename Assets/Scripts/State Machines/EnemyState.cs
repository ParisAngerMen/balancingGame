[System.Serializable]
public class EnemyState
{
    protected EnemyController enemy;
    protected EnemyStateMachine enemyStateMachine;

    public EnemyState(EnemyController enemy, EnemyStateMachine enemyStateMachine)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
    }

    public virtual void EnterState(){}
    public virtual void ExitState(){}
    public virtual void FrameUpdate(){}
    public virtual void PhysicsUpdate(){}
}
