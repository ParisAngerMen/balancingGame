using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AI;

public class ArcherController : EnemyController
{
    public override IEnumerator Attack(GameObject target)
    {
        while (true)
        {
            Debug.Log("Attacking");
            
            if (target == null)
            {
                Debug.Log("Enemy not found");
                if (enemyBase != null)
                {
                    enemyHit = enemyBase;
                    navMeshAgent.destination = enemyBase.transform.position;
                }

                attackTimer = 0f;
                yield return SightCheck();
                yield break;
            }
            
            attackTimer += Time.deltaTime;
            
            Debug.Log("Time to attack: " + attackTimer);
            
            if (attackTimer >= cooldown)
            {
                if (target.GetComponent<BaseScript>() != null)
                {
                    target.GetComponent<BaseScript>().TakeDamage(damage);
                }

                else
                {
                    target.GetComponent<EnemyController>().TakeDamage(damage);
                }

                attackTimer = 0f;
            }
            
            yield return new WaitForSeconds(0.1f);
        }        
    }
}
