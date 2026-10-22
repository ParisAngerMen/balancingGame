using UnityEngine;
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
    public virtual void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType){}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
