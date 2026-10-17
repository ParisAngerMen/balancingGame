using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float range;
    [SerializeField] public float cooldown;
    [SerializeField] public float lootPoints;
    
    [Header("Other Variables")]
    [SerializeField] private float visionRange;
    public float attackTimer = 0f;
    [SerializeField] private float angleRange;
    [SerializeField] private float rotSpeed;

    
    [Header("Other Components")]
    public GameObject enemyHit;
    public NavMeshAgent navMeshAgent;
    public LayerMask enemyMask;

    private Coroutine attackCoroutine;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        if (gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("RedBase");
        }
        
        else if (gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("BlueBase");

        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHit = enemyBase;
        currentHealth = maxHealth;
        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = range;
        
        //navMeshAgent.updateRotation = false;
        navMeshAgent.SetDestination(enemyBase.transform.position);
        StartCoroutine(SightCheck());

    }

    private void Update()
    {
        //Debug.Log("Destination: " + navMeshAgent.destination);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
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

    public IEnumerator SightCheck()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionRange, enemyMask,
                QueryTriggerInteraction.Collide);
            
            Debug.Log("Checking for sight");

            foreach (Collider hit in hitColliders)
            {
                Debug.Log("HitPoint: " + hit.gameObject.name);
                Vector3 hitPoint = hit.transform.position;
                Vector3 targetDirection = hitPoint - transform.position;

                if (Vector3.Distance(transform.position, hitPoint) > 
                    Vector3.Distance(transform.position, navMeshAgent.destination) && 
                    hit.gameObject.CompareTag("Enemy"))
                {
                    yield break;
                } 

                float angle = Vector3.Angle(targetDirection, transform.forward);
                
                if (angle < angleRange)
                {
                    Debug.Log("Object detected");

                    if (attackCoroutine == null)
                    {
                        enemyHit = hit.gameObject;
                        navMeshAgent.SetDestination(hitPoint);
                        attackCoroutine = StartCoroutine(Attack(hit.gameObject));
                    }
                }

                else
                {
                    enemyHit = enemyBase;
                    Debug.Log("Object not found");
                    StopCoroutine(Attack(hit.gameObject));
                    navMeshAgent.SetDestination(enemyBase.transform.position);
                }

            }
            yield return new WaitForSeconds(0.1f);

        }
        
    }

    public virtual IEnumerator Attack(GameObject target)
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
                attackCoroutine = null;
                yield return SightCheck();
                yield break;
            }
            
            Debug.Log("Time to attack: " + attackTimer);
            
            IDamageable damageable = target.GetComponent<IDamageable>();
            Debug.Log(target.name + " taking damage");
            damageable.TakeDamage(damage);
            
            attackTimer = 0f;
            
            yield return new WaitForSeconds(cooldown);
        }
    }

    
}

