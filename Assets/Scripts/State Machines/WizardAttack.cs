using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class WizardAttack : EnemyAttackState
{
    private AudioSource audioSource;
    private Animator animator;
    private float attackRadius;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public WizardAttack(WizardController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        audioSource = enemy.gameObject.GetComponent<AudioSource>();
        animator = enemy.gameObject.GetComponent<Animator>();
        attackRadius = enemy.attackRadius;
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
                RaycastHit[] hits = Physics.SphereCastAll(enemy.enemyHit.transform.position, 
                    attackRadius, Vector3.forward,enemy.range, 
                    enemy.enemyMask, QueryTriggerInteraction.Collide);

                foreach (RaycastHit hit in hits)
                {
                    Debug.Log("Targets: " + hits.Length);
                    
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();

                    if (enemy.attackSound != null)
                    {
                        audioSource.PlayOneShot(enemy.attackSound);
                    }
                
                    damageable.TakeDamage(enemy.damage);
                    enemy.attackTimer = 0;
                }
            }
        }

        else
        {
            enemy.ChangeToWalkState();
        }
        
    }

}
