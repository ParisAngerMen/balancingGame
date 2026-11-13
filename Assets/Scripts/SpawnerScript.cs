using UnityEngine;
using static UnityEngine.Input;

public class SpawnerScript : MonoBehaviour
{
    // Half size of the square area around a spawn zone where units can appear
    private const float SpawnAreaHalfSize = 10f;
    // Free space required around a spawn point
    private const float SpawnCheckRadius = 2f;
    // Random positions tried before spawning anyway
    private const int MaxSpawnAttempts = 30;

    public GameObject[] enemies;

    public Transform blueSpawnZone;
    public Transform redSpawnZone;

    [Header("Materials")]
    [SerializeField] private Material red;
    [SerializeField] private Material blue;

    private LayerMask blueMask;
    private LayerMask redMask;

    public bool canSpawn = false;
    public bool enableAutoSpawn = false;

    public float timeToSpawn = 3f;
    public float spawnTimer = 0f;

    private void Awake()
    {
        blueMask = LayerMask.GetMask("Blue");
        redMask = LayerMask.GetMask("Red");
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            if (GetKeyDown(KeyCode.Alpha1)) SpawnByIndex(0, true);
            if (GetKeyDown(KeyCode.Alpha2)) SpawnByIndex(0, false);
            if (GetKeyDown(KeyCode.Alpha3)) SpawnByIndex(1, true);
            if (GetKeyDown(KeyCode.Alpha4)) SpawnByIndex(1, false);
            if (GetKeyDown(KeyCode.Alpha5)) SpawnByIndex(2, true);
            if (GetKeyDown(KeyCode.Alpha6)) SpawnByIndex(2, false);
            if (GetKeyDown(KeyCode.Alpha7)) SpawnByIndex(3, true);
            if (GetKeyDown(KeyCode.Alpha8)) SpawnByIndex(3, false);
        }

        if (enableAutoSpawn)
        {
            SpawnRedEnemies();
        }
    }

    private void SpawnByIndex(int index, bool isRed)
    {
        if (enemies == null || index < 0 || index >= enemies.Length)
        {
            Debug.LogWarning("Spawner has no enemy assigned at index " + index);
            return;
        }

        SetTeam(enemies[index], isRed);
    }

    /// <summary>
    /// Spawns a copy of the given unit for the chosen team.
    /// </summary>
    public void SetTeam(GameObject enemy, bool isRed)
    {
        Transform spawnZone = isRed ? redSpawnZone : blueSpawnZone;

        if (enemy == null || spawnZone == null)
        {
            Debug.LogWarning("Spawner is missing the unit prefab or the spawn zone");
            return;
        }

        GameObject clone = Instantiate(enemy, CalculateSpawnPoint(spawnZone), Quaternion.identity);
        clone.name = enemy.name;

        // The team is set on the spawned copy, never on the prefab asset itself.
        // EnemyController and MaterialAdder read the layer in Start, which runs after this.
        clone.layer = LayerMask.NameToLayer(isRed ? "Red" : "Blue");

        if (clone.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.enemyMask = isRed ? blueMask : redMask;
        }
    }

    private Vector3 CalculateSpawnPoint(Transform spawnZone)
    {
        Vector3 center = spawnZone.position;
        Vector3 candidate = center;
        int occupiedMask = blueMask | redMask;

        // Bounded number of tries: a crowded zone can no longer freeze or crash the game
        for (int attempt = 0; attempt < MaxSpawnAttempts; attempt++)
        {
            candidate = new Vector3(
                Random.Range(center.x - SpawnAreaHalfSize, center.x + SpawnAreaHalfSize),
                center.y,
                Random.Range(center.z - SpawnAreaHalfSize, center.z + SpawnAreaHalfSize));

            // Free when no unit or base overlaps the spot
            if (!Physics.CheckSphere(candidate, SpawnCheckRadius, occupiedMask, QueryTriggerInteraction.Collide))
            {
                return candidate;
            }
        }

        return candidate;
    }

    void SpawnRedEnemies()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= timeToSpawn)
        {
            spawnTimer = 0f;

            if (enemies != null && enemies.Length > 0)
            {
                int randomEnemy = Random.Range(0, enemies.Length);
                SetTeam(enemies[randomEnemy], true);
            }
        }
    }
}
