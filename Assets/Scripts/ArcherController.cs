using UnityEngine;

public class ArcherController : EnemyController
{
    [Header("Archer")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float turnspeed = 10f;

    public float TurnSpeed => turnspeed;

    public override void SetAttack()
    {
        AttackState = new ArcherAttack(this, StateMachine);
    }

    public void Shoot(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow Prefab is not assigned on " + name);
            return;
        }

        Vector3 aimPoint = target.transform.position + Vector3.up;
        Vector3 spawn = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        Vector3 direction = aimPoint - spawn;

        // LookRotation needs a non-zero direction
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = transform.forward;
        }

        GameObject arrowObj = Instantiate(arrowPrefab, spawn, Quaternion.LookRotation(direction));
        Projectile arrow = arrowObj.GetComponent<Projectile>();

        if (arrow == null)
        {
            Debug.LogError("Arrow prefab has no Projectile script on it");
            Destroy(arrowObj);
            return;
        }

        arrow.Init(damage, gameObject.layer, direction);
    }
}
