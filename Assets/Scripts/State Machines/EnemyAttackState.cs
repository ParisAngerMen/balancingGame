using UnityEngine;

[System.Serializable]
public class EnemyAttackState : EnemyState
{
    // Minimum distance the target has to move before the path is recalculated
    private const float RepathDistanceSqr = 0.01f;

    private AudioSource audioSource;
    private Animator animator;

    private Vector3 lastDestination;
    private bool hasDestination;

    public EnemyAttackState(EnemyController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        audioSource = enemy.gameObject.GetComponent<AudioSource>();
        animator = enemy.gameObject.GetComponent<Animator>();
    }

    public override void EnterState()
    {
        Debug.Log("Enter attack state");
        hasDestination = false;

        if (animator != null)
        {
            animator.SetBool("attack", true);
        }

        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.updateRotation = false;
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exit attack state");

        if (animator != null)
        {
            animator.SetBool("attack", false);
        }

        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.updateRotation = true;
        }
    }

    public override void FrameUpdate()
    {
        GameObject target = enemy.enemyHit;

        if (target == null)
        {
            // Target died or disappeared: go back to walking towards the enemy base
            enemy.enemyHit = enemy.enemyBase;
            enemy.ChangeToWalkState();
            return;
        }

        ChaseTarget(target);
        FaceTarget(target);

        // The cooldown keeps running while approaching, but the hit only lands in range
        enemy.attackTimer = Mathf.Min(enemy.attackTimer + Time.deltaTime, enemy.cooldown);

        if (enemy.attackTimer >= enemy.cooldown && enemy.IsTargetInRange(target))
        {
            PerformAttack(target);
            enemy.attackTimer = 0f;
        }
    }

    private void ChaseTarget(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;

        // Only ask for a new path when the target has actually moved
        if (hasDestination && (targetPosition - lastDestination).sqrMagnitude <= RepathDistanceSqr)
        {
            return;
        }

        if (enemy.MoveTo(targetPosition))
        {
            lastDestination = targetPosition;
            hasDestination = true;
        }
    }

    /// <summary>
    /// Turns the unit towards the target, staying upright.
    /// </summary>
    protected virtual void FaceTarget(GameObject target)
    {
        Vector3 direction = target.transform.position - enemy.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            enemy.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// Executes one attack. Called when the cooldown is over and the target is in range.
    /// </summary>
    protected virtual void PerformAttack(GameObject target)
    {
        IDamageable damageable = target.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        PlayAttackSound();
        damageable.TakeDamage(enemy.damage);
    }

    protected void PlayAttackSound()
    {
        if (enemy.attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(enemy.attackSound);
        }
    }
}
