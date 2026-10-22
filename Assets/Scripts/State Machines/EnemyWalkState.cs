using UnityEngine;
using UnityEngine.AI;
[System.Serializable]
public class EnemyWalkState : EnemyState
{
    private NavMeshAgent navMeshAgent;
    private GameObject enemyBase;
    private Animator animator;
    public EnemyWalkState(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        navMeshAgent = enemy.gameObject.GetComponent<NavMeshAgent>();
        animator = enemy.gameObject.GetComponent<Animator>();
    }

    public override void EnterState()
    {
        enemyBase = enemy.enemyBase;
        Debug.Log("Enemy Base: " + enemyBase);
        // Change animations
        navMeshAgent.SetDestination(enemyBase.transform.position);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }
}
