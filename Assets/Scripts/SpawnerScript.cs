using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Input;

public class SpawnerScript : MonoBehaviour
{
    public GameObject melee;
    
    public Transform blueSpawnZone;
    public Transform redSpawnZone;

    private int enemySpawnNumber = 0;
    private Vector3 spawnZone;
    
    [Header("Materials")]
    [SerializeField] private Material red;
    [SerializeField] private Material blue;
    private MeshRenderer meshRenderer;

    private LayerMask blueMask;
    private LayerMask redMask;
    
    //private float spawnZoneX;
    //private float spawnZoneZ;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blueMask = LayerMask.GetMask("Blue");
        redMask = LayerMask.GetMask("Red");
    }

    // Update is called once per frame
    void Update()
    {
        if (GetKeyDown(KeyCode.Alpha1))
        {
            SetTeam(melee, true);
        }
        
        if (GetKeyDown(KeyCode.Alpha2))
        {
            SetTeam(melee, false);
        }
    }

    void SpawnEnemy(GameObject enemy,  Vector3 spawnZone)
    {
        enemySpawnNumber++;
        enemy.name = enemySpawnNumber.ToString();

        Instantiate(enemy, spawnZone, Quaternion.identity);

    }

    void SetTeam(GameObject enemy, bool isRed)
    {
        if (isRed)
        {
            enemy.GetComponent<EnemyController>().enemyMask = blueMask;
            Debug.Log("enemy mask: " + enemy.GetComponent<EnemyController>().enemyMask);
            enemy.layer = LayerMask.NameToLayer("Red");
            meshRenderer = enemy.GetComponent<MeshRenderer>();
            
            meshRenderer.material = red;
            CalculateSpawnPoint(melee, redSpawnZone);
        }

        else
        {
            enemy.GetComponent<EnemyController>().enemyMask = redMask;  
            Debug.Log("enemy mask: " + enemy.GetComponent<EnemyController>().enemyMask);
            enemy.layer = LayerMask.NameToLayer("Blue");
            meshRenderer = enemy.GetComponent<MeshRenderer>();
            
            meshRenderer.material = blue;
            CalculateSpawnPoint(melee, blueSpawnZone);

        }
    }

    void CalculateSpawnPoint(GameObject enemy, Transform spawnZone)
    {
        float spawnZoneX = Random.Range(spawnZone.position.x + 10f, spawnZone.position.x - 10f);   
        float spawnZoneZ = Random.Range(spawnZone.position.z + 10f, spawnZone.position.z - 10f);   
        Vector3 randomSpawnZone = new Vector3(spawnZoneX, spawnZone.position.y, spawnZoneZ);
        
        if (!Physics.SphereCast(randomSpawnZone, 2f, Vector3.forward, out RaycastHit hit))
        {
            SpawnEnemy(enemy, randomSpawnZone);
        }

        else
        {
            CalculateSpawnPoint(enemy, spawnZone);
        }
    }
}
