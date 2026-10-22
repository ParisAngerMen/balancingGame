using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    private AudioSource audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EnemyAttackState(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        audioSource = enemy.gameObject.GetComponent<AudioSource>();
    }

    public override void EnterState()
    {
        enemy.navMeshAgent.SetDestination(enemy.enemyHit.transform.position);
        enemy.navMeshAgent.updateRotation = false;
    }

    public override void ExitState()
    {
        enemy.navMeshAgent.SetDestination(enemy.enemyBase.transform.position);
        enemy.navMeshAgent.updateRotation = true;
    }

    public override void FrameUpdate()
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
    

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationTriggerEvent(EnemyController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
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
