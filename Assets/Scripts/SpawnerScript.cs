using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Input;

public class SpawnerScript : MonoBehaviour
{
    public GameObject[] enemies;
    
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
    
    public bool canSpawn = false;
    public bool enableAutoSpawn = false;

    public float timeToSpawn = 3f;
    public float spawnTimer = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blueMask = LayerMask.GetMask("Blue");
        redMask = LayerMask.GetMask("Red");
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            if (GetKeyDown(KeyCode.Alpha1))
            {
                SetTeam(enemies[0], true);
            }
        
            if (GetKeyDown(KeyCode.Alpha2))
            {
                SetTeam(enemies[0], false);
            }        
        
            if (GetKeyDown(KeyCode.Alpha3))
            {
                SetTeam(enemies[1], true);
            }
        
            if (GetKeyDown(KeyCode.Alpha4))
            {
                SetTeam(enemies[1], false);
            }

            if (GetKeyDown(KeyCode.Alpha5))
            {
                SetTeam(enemies[2], true);
            }            
            
            if (GetKeyDown(KeyCode.Alpha6))
            {
                SetTeam(enemies[2], false);
            }            
            
            if (GetKeyDown(KeyCode.Alpha7))
            {
                SetTeam(enemies[3], true);
            }            
            
            if (GetKeyDown(KeyCode.Alpha8))
            {
                SetTeam(enemies[3], false);
            }
        }

        if (enableAutoSpawn)
        {
            SpawnRedEnemies();
        }
        
    }

    void SpawnEnemy(GameObject enemy,  Vector3 spawnZone)
    {
        var clone = Instantiate(enemy, spawnZone, Quaternion.identity);
        clone.name = enemy.name;
    }

    public void SetTeam(GameObject enemy, bool isRed)
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        MaterialAdder materialAdder = enemy.GetComponent<MaterialAdder>();
        
        if (isRed)
        {
            enemyController.enemyMask = blueMask;
            enemy.layer = LayerMask.NameToLayer("Red");
            
            CalculateSpawnPoint(enemy, redSpawnZone);
        }

        else
        {
            enemyController.enemyMask = redMask;  
            enemy.layer = LayerMask.NameToLayer("Blue");

            CalculateSpawnPoint(enemy, blueSpawnZone);

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

    void SpawnRedEnemies()
    {
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= timeToSpawn)
        {
            int randomEnemy = Random.Range(0, enemies.Length);
            SetTeam(enemies[randomEnemy].gameObject, true);
            spawnTimer = 0f;
        }
    }
}
