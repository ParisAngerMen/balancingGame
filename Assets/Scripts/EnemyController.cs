using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IDamageable
{
    // How often the unit scans for targets (seconds)
    private const float SightInterval = 0.1f;

    // Objects / Transforms
    [Header("Enemy Base")]
    public GameObject enemyBase;

    // Float
    [Header("Balance Variables")]
    [SerializeField] private float speed;
    public float Speed => speed;
    public float invunerableSeconds = 1f;
    private float invunerableTimer;
    private bool canBeDamaged = true;
    private bool isDead;
    [field: SerializeField] public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    [SerializeField] public float damage;
    [SerializeField] public float range;
    [SerializeField] public float cooldown;
    [SerializeField] public float lootPoints;
    [SerializeField] public float cost;

    [Header("Other Variables")]
    [SerializeField] private float visionRange;
    public float attackTimer = 0f;
    [SerializeField] private float angleRange;
    [SerializeField] private float rotSpeed;
    public GameObject enemyHit;
    public NavMeshAgent navMeshAgent;
    public LayerMask enemyMask;

    public EnemyStateMachine StateMachine { get; set; }
    public EnemyState WalkState { get; set; }
    [Header("Enemy States")]
    [SerializeField] public EnemyState AttackState;

    [Header("UI")]
    [SerializeField] private Slider healthBar;

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] public AudioClip attackSound;
    [SerializeField] public AudioClip hurtSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // The state machine has to exist before the states that receive it are created
        StateMachine = new EnemyStateMachine();
        WalkState = new EnemyWalkState(this, StateMachine);
        SetAttack();
    }

    /// <summary>
    /// Overwrites the balance values of this unit (used to load them from the Google Sheet).
    /// </summary>
    public void ApplyStats(float newMaxHealth, float newDamage, float newRange, float newCooldown,
        float newLootPoints, float newCost, float newSpeed)
    {
        maxHealth = newMaxHealth;
        damage = newDamage;
        range = newRange;
        cooldown = newCooldown;
        lootPoints = newLootPoints;
        cost = newCost;
        speed = newSpeed;
    }

    public virtual void SetAttack()
    {
        AttackState = new EnemyAttackState(this, StateMachine);
    }

    public void ChangeToWalkState()
    {
        StateMachine.ChangeState(WalkState);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // The team (layer) is assigned by the spawner right after instantiating,
        // so the enemy base is resolved here instead of in Awake
        FindEnemyBase();

        enemyHit = enemyBase;
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.stoppingDistance = range;
        }

        StateMachine.Initialize(WalkState);
        StartSight();
    }

    private void FindEnemyBase()
    {
        if (gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("RedBase");
        }

        else if (gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("BlueBase");
        }

        if (enemyBase == null)
        {
            Debug.LogWarning(name + " could not find an enemy base to walk to");
        }
    }

    private void Update()
    {
        if (StateMachine.CurrentEnemyState != null)
        {
            StateMachine.CurrentEnemyState.FrameUpdate();
        }

        if (!canBeDamaged)
        {
            invunerableTimer += Time.deltaTime;
            if (invunerableTimer >= invunerableSeconds)
            {
                invunerableTimer = 0f;
                canBeDamaged = true;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || !canBeDamaged)
        {
            return;
        }

        currentHealth -= damage;

        // With no invulnerability time configured every hit counts,
        // even several hits landing on the same frame
        if (invunerableSeconds > 0f)
        {
            canBeDamaged = false;
            invunerableTimer = 0f;
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (hitEffectPrefab != null)
        {
            var fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
        }

        if (hurtSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Destroy() only takes effect at the end of the frame, so make sure
        // the loot is not awarded more than once
        if (isDead)
        {
            return;
        }

        isDead = true;
        StopAllCoroutines();

        if (PointManager.instance != null)
        {
            // Killing a red unit rewards blue and vice versa
            bool rewardRed = gameObject.layer != LayerMask.NameToLayer("Red");
            PointManager.instance.AddPoints(lootPoints, rewardRed);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Sends the agent to a position. Returns false when the agent cannot move
    /// (disabled or not placed on the NavMesh).
    /// </summary>
    public bool MoveTo(Vector3 destination)
    {
        if (!AgentIsReady())
        {
            return false;
        }

        return navMeshAgent.SetDestination(destination);
    }

    /// <summary>
    /// True when the target is within attack range, or when the agent has already
    /// got as close to it as it can (for example a base carved out of the NavMesh).
    /// </summary>
    public bool IsTargetInRange(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 origin = transform.position;
        Vector3 closestPoint = target.transform.position;

        if (target.TryGetComponent(out Collider targetCollider))
        {
            bool supportsClosestPoint = !(targetCollider is MeshCollider meshCollider) || meshCollider.convex;
            closestPoint = supportsClosestPoint
                ? targetCollider.ClosestPoint(origin)
                : targetCollider.bounds.ClosestPoint(origin);
        }

        Vector3 offset = closestPoint - origin;
        offset.y = 0f;

        if (offset.sqrMagnitude <= range * range)
        {
            return true;
        }

        return AgentIsReady()
               && !navMeshAgent.pathPending
               && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }   
    private bool AgentIsReady()
    {
        return navMeshAgent != null && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh;
    }

    public void StartSight()
    {
        StartCoroutine(SightCheck());
    }

    public IEnumerator SightCheck()
    {
        var wait = new WaitForSeconds(SightInterval);

        while (true)
        {
            bool needTarget = enemyHit == null || enemyHit == enemyBase;

            if (needTarget)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionRange, enemyMask,
                    QueryTriggerInteraction.Collide);

                float closestDist = float.MaxValue;
                GameObject closest = null;

                foreach (Collider hit in hitColliders)
                {
                    if (hit == null || hit.gameObject == gameObject)
                    {
                        continue;
                    }

                    Vector3 targetDirection = hit.transform.position - transform.position;
                    float angle = Vector3.Angle(targetDirection, transform.forward);
                    float distance = targetDirection.magnitude;

                    // Real distance to this candidate, not its world-position magnitude
                    if (angle < angleRange && distance < closestDist)
                    {
                        closestDist = distance;
                        closest = hit.gameObject;
                    }
                }

                if (closest != null)
                {
                    enemyHit = closest;
                    MoveTo(enemyHit.transform.position);

                    if (StateMachine.CurrentEnemyState != AttackState)
                    {
                        StateMachine.ChangeState(AttackState);
                    }
                }
            }

            yield return wait;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        // The agent is only available (and on the NavMesh) while playing
        if (Application.isPlaying && AgentIsReady())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(navMeshAgent.destination, Vector3.one);
            Gizmos.DrawLine(transform.position, navMeshAgent.destination);
        }
    }
}
