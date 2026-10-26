using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ArcherAttack : EnemyAttackState
{
    private Vector3 position;
    private NavMeshAgent navMeshAgent;
    private float turnspeed = 10f;
    private GameObject arrowPrefab;
    
    public ArcherAttack(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        position = enemy.transform.position;
        navMeshAgent = enemy.GetComponent<NavMeshAgent>();

    }

    public override void FrameUpdate()
    {
        Debug.Log("Enemy hit: " + enemy.enemyHit);

        if (enemy.enemyHit)
        {
            enemy.gameObject.transform.LookAt(enemy.enemyHit.transform);
        
            Vector3 dir = enemy.enemyHit.transform.position - position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                enemy.transform.rotation = Quaternion.Slerp(
                    enemy.transform.rotation,
                    Quaternion.LookRotation(dir),
                    turnspeed * Time.deltaTime);
            }

            enemy.attackTimer += Time.deltaTime;
        
            if (enemy.attackTimer >= enemy.cooldown)
            {
                enemy.GetComponent<ArcherController>().Shoot(enemy.enemyHit);
                enemy.attackTimer = 0f;
            }
        }

        else
        {
            enemy.ChangeToWalkState();
        }

    }
    
}
