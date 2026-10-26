using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour, IDamageable
{
    
    // Objects / Transforms
    [Header("Enemy Base")]
    public GameObject enemyBase;
    
    // Bools
    // public bool isBlue;
    
    // Float
    [Header("Balance Variables")]
    [SerializeField] private float speed;
    [field: SerializeField] public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    [SerializeField] public float damage;
    [SerializeField] public float range;
    [SerializeField] public float cooldown;
    [SerializeField] public float lootPoints;
    
    [Header("Other Variables")]
    [SerializeField] private float visionRange;
    public float attackTimer = 0f;
    private bool isAttacking = false;
    [SerializeField] private float angleRange;
    [SerializeField] private float rotSpeed;
    public GameObject enemyHit;
    public NavMeshAgent navMeshAgent;
    public LayerMask enemyMask;
    private Animator animator;

    [Header("Enemy States")] 
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyState WalkState { get; set; }
    [SerializeField] public EnemyState AttackState;

    
    [Header("UI")]
    [SerializeField]private Slider healthBar;

    [Header("Effects")]
    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] public AudioClip attackSound;
    [SerializeField] public AudioClip hurtSound;
    private AudioSource audioSource;
    protected Coroutine attackCoroutine;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        WalkState = new EnemyWalkState(this, StateMachine);
        SetAttack();
        
        if (gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("RedBase");
        }
        
        else if (gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("BlueBase");
        }
        
        StateMachine = new EnemyStateMachine();
        
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
        enemyHit = enemyBase;
        currentHealth = maxHealth;
        
        Debug.Log("Enemy Base: " + enemyBase);

        StateMachine.Initialize(WalkState);
        
        if(healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = range;
        StartSight();
    }
    
    private void Update()
    {
        Debug.Log("EnemyState: " + StateMachine.CurrentEnemyState);
        StateMachine.CurrentEnemyState.FrameUpdate();
    }
    

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }


        if (currentHealth <= 0)
        {
            Die();
        }

        if (hitEffectPrefab != null)
        {
            var fx = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx.gameObject, fx.main.duration + fx.main.startLifetime.constantMax);
        }

        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

    }

    public void Die()
    {
        StopAllCoroutines();

        if (gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            PointManager.instance.AddPoints(lootPoints, false);
        }

        else
        {
            PointManager.instance.AddPoints(lootPoints, true);
        }
        
        Destroy(gameObject);
    }

    public void StartSight()
    {
        StartCoroutine(SightCheck());
    }

    public IEnumerator SightCheck()
    {
        while (true)
        {
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionRange, enemyMask,
                QueryTriggerInteraction.Collide);
            
            foreach (Collider hit in hitColliders)
            {
                Vector3 hitPoint = hit.transform.position;
                Vector3 targetDirection = hitPoint - transform.position;

                float angle = Vector3.Angle(targetDirection, transform.forward);
                
                if (angle < angleRange)
                {
                    Debug.Log("Angle: " + angle);
                    enemyHit = hit.gameObject;
                    StateMachine.ChangeState(AttackState);
                }
            }
            yield return new WaitForSeconds(0.1f);

        }
        
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        if (navMeshAgent.destination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(navMeshAgent.destination, Vector3.one);
            Gizmos.DrawLine(transform.position, navMeshAgent.destination);
        }

    }
}

