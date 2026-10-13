using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Objects / Transforms
    private Transform enemyBase;
    
    // Bools
    // public bool isBlue;
    
    // Float
    [Header("Balance Variables")]
    [SerializeField] private float speed;
    public float health;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float cooldown;
    
    [Header("Other Variables")]
    [SerializeField] private float visionRange;
    private float currentHealth;
    private float attackTimer = 0f;
    [SerializeField] private float angleRange;
    [SerializeField] private float rotSpeed;
    
    // Component ref
    private NavMeshAgent navMeshAgent;
    public LayerMask enemyMask;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        if (gameObject.layer == LayerMask.NameToLayer("Blue"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("RedBase").transform;
        }
        
        else if (gameObject.layer == LayerMask.NameToLayer("Red"))
        {
            enemyBase = GameObject.FindGameObjectWithTag("BlueBase").transform;

        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = health;
        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = range;
        
        //navMeshAgent.updateRotation = false;
        navMeshAgent.SetDestination(enemyBase.position);
        StartCoroutine(SightCheck());

    }

    private void Update()
    {
        Debug.Log("Destination: " + navMeshAgent.destination);
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

    private void Die()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private IEnumerator SightCheck()
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
                    (!hit.CompareTag("BlueBase") || !hit.CompareTag("BlueBase"))) yield break; 

                float angle = Vector3.Angle(targetDirection, transform.forward);
                
                if (angle < angleRange)
                {
                    Debug.Log("Object detected");
                    
                    navMeshAgent.SetDestination(hitPoint);
                    StartCoroutine(Attack(hit.gameObject));
                }

                else
                {
                    Debug.Log("Object not found");
                    
                    StopCoroutine(Attack(hit.gameObject));
                    navMeshAgent.SetDestination(enemyBase.transform.position);
                }

            }
            yield return new WaitForSeconds(0.1f);

        }
        
    }

    private IEnumerator Attack(GameObject target)
    {
        while (true)
        {
            Debug.Log("Attacking");
            
            if (target == null)
            {
                Debug.Log("Enemy not found");
                if (enemyBase != null)
                {
                    navMeshAgent.destination = enemyBase.transform.position;
                }
                
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
