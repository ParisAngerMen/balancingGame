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

    public override void SetAttack()
    {
        AttackState = new ArcherAttack(this, StateMachine);
    }
    
    public override IEnumerator Attack(GameObject target)
    {
        while (true)
        {
            if (target == null)
            {
                if (enemyBase != null)
                {
                    enemyHit = enemyBase;
                    navMeshAgent.SetDestination(enemyBase.transform.position);
                }

                attackTimer = 0f;
                attackCoroutine = null;
                Debug.Log("Seeing no enemy");
                yield return SightCheck();
                yield break;
            }

            Vector3 dir = target.transform.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    turnspeed * Time.deltaTime);
            }

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= navMeshAgent.stoppingDistance + 2f)
            {
                attackTimer += Time.deltaTime;

                if (attackTimer >= cooldown)
                {
                    Shoot(target);
                    attackTimer = 0f;
                }
            }

            yield return null;
        }
    }

    private float nextDebug;

    private void Update()
    {
        if (enemyBase == null)
        {
            if (Time.time > nextDebug)
            {
                nextDebug = Time.time + 1f;
                Debug.LogError(name + ": enemyBase es NULL (revisa el tag de la base)");
            }
            return;
        }

        Collider col = enemyBase.GetComponentInChildren<Collider>();
        Vector3 closest = col != null ? col.ClosestPoint(transform.position) : enemyBase.transform.position;
        float dist = Vector3.Distance(transform.position, closest);
        float allowed = navMeshAgent.stoppingDistance + 2f;

        if (Time.time > nextDebug)
        {
            nextDebug = Time.time + 1f;
            Debug.Log($"{name} -> base: {enemyBase.name} | collider: {(col != null ? col.GetType().Name : "NINGUNO")} " +
                      $"| dist: {dist:F1} / permitido: {allowed:F1} | path: {navMeshAgent.pathStatus} " +
                      $"| attackCoroutine null: {attackCoroutine == null}");
        }

        if (attackCoroutine == null && dist <= allowed)
        {
            enemyHit = enemyBase;
            attackCoroutine = StartCoroutine(Attack(enemyBase));
        }
    }
    private void Shoot(GameObject target)
    {
        Debug.Log("Shoot called");

        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow Prefab is not assigned on " + name);
            return;
        }

        Vector3 aimPoint = target.transform.position + Vector3.up;
        Vector3 spawn = firePoint != null ? firePoint.position : transform.position + Vector3.up;

        GameObject arrowObj = Instantiate(arrowPrefab, spawn, Quaternion.LookRotation(aimPoint - spawn));
        Projectile arrow = arrowObj.GetComponent<Projectile>();

        if (arrow == null)
        {
            Debug.LogError("Arrow prefab has no Projectile script on it");
            return;
        }

        arrow.Init(damage, gameObject.layer, aimPoint - spawn);
    }
}
