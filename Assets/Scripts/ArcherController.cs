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
    
    public void Shoot(GameObject target)
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
