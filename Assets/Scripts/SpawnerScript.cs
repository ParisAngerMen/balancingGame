using UnityEngine;
using static UnityEngine.Input;

public class SpawnerScript : MonoBehaviour
{
    public GameObject blueMelee;
    public GameObject redMelee;
    
    public Transform blueSpawnZone;
    public Transform redSpawnZone;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetKeyDown(KeyCode.Alpha1))
        {
            SpawnEnemy(redMelee, redSpawnZone);
        }
        
        if (GetKeyDown(KeyCode.Alpha2))
        {
            SpawnEnemy(blueMelee, blueSpawnZone);
        }
    }

    void SpawnEnemy(GameObject enemy,  Transform spawnZone)
    {
        Instantiate(enemy, spawnZone.position, spawnZone.rotation);
    }
}
