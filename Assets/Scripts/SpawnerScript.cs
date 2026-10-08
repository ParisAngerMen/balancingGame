using UnityEngine;
using static UnityEngine.Input;

public class SpawnerScript : MonoBehaviour
{
    public GameObject blueMelee;
    public GameObject redMelee;
    
    public Transform blueSpawnZone;
    public Transform redSpawnZone;

    private int enemySpawnNumber = 0;
    private Vector3 spawnZone;

    //private float spawnZoneX;
    //private float spawnZoneZ;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetKeyDown(KeyCode.Alpha1))
        {
            CalculateSpawnPoint(redMelee, redSpawnZone);
        }
        
        if (GetKeyDown(KeyCode.Alpha2))
        {
            CalculateSpawnPoint(blueMelee, blueSpawnZone);
        }
    }

    void SpawnEnemy(GameObject enemy,  Vector3 spawnZone)
    {
        enemySpawnNumber++;
        enemy.name = enemySpawnNumber.ToString();

        Instantiate(enemy, spawnZone, Quaternion.identity);

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
