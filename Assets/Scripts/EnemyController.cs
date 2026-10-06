using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Objects / Transforms
    [SerializeField] private Transform enemyBase;
    
    // Bools
    public bool isBlue;
    
    // Float
    [Header("Variables Balanceo")]
    public float maxHealth;
    [SerializeField] private float speed;
    [SerializeField] private float visionRange;
    
    private float currentHealth;
    [SerializeField] private float angleRange;
    
    // Component ref
    private NavMeshAgent navMeshAgent;
    public LayerMask enemyMask;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        navMeshAgent.speed = speed;
        navMeshAgent.SetDestination(enemyBase.position);
        //StartCoroutine(SightCheck());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Death logico
    }

    private IEnumerator SightCheck()
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
                    navMeshAgent.SetDestination(hit.transform.position);
                    
                    Debug.Log("Sight");
                    Debug.Log("Angle: "  + angle);
                }

                else
                {
                    Debug.Log("Angle: "  + angle);
                }

            }
            yield return new WaitForSeconds(0.1f);

        }
        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, visionRange);

    }
    
}
