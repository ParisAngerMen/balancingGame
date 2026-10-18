using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AI;

public class ArcherController : EnemyController
{
    [Header("Archer")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float turnspeed = 10f;
    private Vector3 aimspot;

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
                    navMeshAgent.SetDestination(enemyBase.transform.position);
                }

                attackTimer = 0f;
                attackCoroutine = null;
                yield break;
            }

            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnspeed * Time.deltaTime);
            }

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= navMeshAgent.stoppingDistance + 1f)
            {
                attackTimer += Time.deltaTime;

                if (attackTimer >= cooldown)
                {
                  Shoot(target);
                  attackTimer = 0f;
                }

            }

            yield return null;
           

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

    private void Shoot(GameObject target)
    {
        Vector3 aimPoint = target.transform.position + Vector3.up;
        Vector3 spawn = firePoint != null ? firePoint.position : transform.position + Vector3.up;

        Projectile arrow = Instantiate(arrowPrefab, spawn, Quaternion.LookRotation(aimPoint - spawn)).GetComponent<Projectile>();
        arrow.Init(damage, gameObject.layer, aimPoint - spawn);
    }
}
