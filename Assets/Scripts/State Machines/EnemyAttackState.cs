using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    private AudioSource audioSource;
    private Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyAttackState(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        audioSource = enemy.gameObject.GetComponent<AudioSource>();
        animator = enemy.gameObject.GetComponent<Animator>();

    }

    public override void EnterState()
    {
        Debug.Log("Enter attack state");
        enemy.navMeshAgent.SetDestination(enemy.enemyHit.transform.position);
        animator.SetBool("attack", true);
        enemy.navMeshAgent.updateRotation = false;
    }

    public override void ExitState()
    {
        Debug.Log("Exit attack state");
        enemy.navMeshAgent.SetDestination(enemy.enemyBase.transform.position);
        animator.SetBool("attack", false);
        enemy.navMeshAgent.updateRotation = true;
    }

    public override void FrameUpdate()
    {
        Debug.Log("Enemy hit: " + enemy.enemyHit);

        if (enemy.enemyHit != null)
        {
            enemy.gameObject.transform.LookAt(enemy.enemyHit.transform);
        
            enemy.attackTimer += Time.deltaTime;
            Debug.Log("Time to attack: " + enemy.attackTimer);

            if (enemy.attackTimer >= enemy.cooldown)
            {
                IDamageable damageable = enemy.enemyHit.GetComponent<IDamageable>();

                if (enemy.attackSound != null)
                {
                    audioSource.PlayOneShot(enemy.attackSound);
                }

                damageable.TakeDamage(enemy.damage);
                enemy.attackTimer = 0;
            }
        }

        else
        {
            enemy.ChangeToWalkState();
        }
        
        
    }
    

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
/*
    
    public virtual IEnumerator Attack(GameObject target)
    {
        while (true)
        {
            if (target == null)
            {
                if (enemyBase != null)
                {
                    enemyHit = enemyBase;
                    navMeshAgent.destination = enemyBase.transform.position;
                }

                attackTimer = 0f;
                isAttacking = false;
                attackCoroutine = null;
                animator.SetBool("attack", false);
                navMeshAgent.updateRotation = true;
                yield return SightCheck();
                yield break;
            }
            
            
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            damageable.TakeDamage(damage);
            
            attackTimer = 0f;
            
            yield return new WaitForSeconds(cooldown);
        }
    }
    */
}
