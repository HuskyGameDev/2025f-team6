using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField]
    private List<GameObject> obstaclePrefabs;

    [SerializeField]
    private List<GameObject> coinPrefabs;

    [SerializeField]
    private List<GameObject> powerupPrefabs;

    [SerializeField]
    private float minSpawnInterval = 1f;

    [SerializeField]
    private float maxSpawnInterval = 5f;

    [SerializeField]
    private bool spawnOnStart = true;

    [SerializeField]
    private int maxCoinPerLane = 3;

    [SerializeField]
    private float coinSpawnDelay = .2f;

    [SerializeField]
    private float powerupSpawnDelay = 15;

    [Header("Progressive Difficulty Settings")]
    [SerializeField]
    private bool enableProgressiveDifficulty = true;

    [SerializeField]
    private float timeToMaxDifficulty = 300f; // 5 minutes to reach max difficulty

    [SerializeField]
    private float minSpawnIntervalLimit = 0.5f; // Fastest spawn rate

    [SerializeField]
    private float maxSpawnIntervalLimit = 2f; // Fastest spawn rate

    [SerializeField]
    private float speedMultiplierLimit = 2f; // Max 2x original speed

    [Header("Spawn Positions")]
    [SerializeField]
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-2f, 8f, 0f),
        new Vector3(0f, 8f, 0f),
        new Vector3(2f, 8f, 0f),
        new Vector3(4f, 8f, 0f),
    };

    [Header("Obstacle Pooling (Optional)")]
    [SerializeField]
    private bool useObjectPooling = true;

    [SerializeField]
    private int poolSize = 10;

    private Coroutine spawnCoroutine;
    private Dictionary<GameObject, Queue<GameObject>> obstaclePools;
    private Dictionary<GameObject, Queue<GameObject>> coinPools;
    private Dictionary<GameObject, Queue<GameObject>> powerupPools;

    // Progressive difficulty tracking
    private float gameTime = 0f;
    private float difficultyProgress = 0f;
    private float currentMinSpawnInterval;
    private float currentMaxSpawnInterval;
    private float currentSpeedMultiplier = 1f;

    // Global Variables
    private bool powerupCanSpawn = true;

    // Event system to notify obstacles of speed changes
    public System.Action<float> OnSpeedMultiplierChanged;

    void Start()
    {
        // Initialize current spawn intervals
        currentMinSpawnInterval = minSpawnInterval;
        currentMaxSpawnInterval = maxSpawnInterval;

        if (useObjectPooling)
        {
            InitializePools();
        }

        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        if (enableProgressiveDifficulty)
        {
            UpdateProgressiveDifficulty();
        }
    }

    private void UpdateProgressiveDifficulty()
    {
        // Update game time
        gameTime += Time.deltaTime;

        // Calculate difficulty progress (0 to 1)
        difficultyProgress = Mathf.Clamp01(gameTime / timeToMaxDifficulty);

        // Update spawn intervals
        currentMinSpawnInterval = Mathf.Lerp(
            minSpawnInterval,
            minSpawnIntervalLimit,
            difficultyProgress
        );
        currentMaxSpawnInterval = Mathf.Lerp(
            maxSpawnInterval,
            maxSpawnIntervalLimit,
            difficultyProgress
        );

        // Update speed multiplier
        float newSpeedMultiplier = Mathf.Lerp(1f, speedMultiplierLimit, difficultyProgress);

        // Only notify if multiplier changed significantly
        if (Mathf.Abs(newSpeedMultiplier - currentSpeedMultiplier) > 0.01f)
        {
            currentSpeedMultiplier = newSpeedMultiplier;
            OnSpeedMultiplierChanged?.Invoke(currentSpeedMultiplier);
        }
    }

    // Add this method to get current difficulty info (useful for UI)
    public float GetDifficultyProgress()
    {
        return difficultyProgress;
    }

    public float GetCurrentSpeedMultiplier()
    {
        return currentSpeedMultiplier;
    }

    // Reset difficulty (call this when game restarts)
    public void ResetDifficulty()
    {
        gameTime = 0f;
        difficultyProgress = 0f;
        currentMinSpawnInterval = minSpawnInterval;
        currentMaxSpawnInterval = maxSpawnInterval;
        currentSpeedMultiplier = 1f;
    }

    private void InitializePools()
    {
        obstaclePools = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (GameObject prefab in obstaclePrefabs)
        {
            if (prefab != null)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject obstacle = CreateObstacleInstance(prefab);
                    obstacle.SetActive(false);
                    objectQueue.Enqueue(obstacle);
                }

                obstaclePools[prefab] = objectQueue;
            }
        }

        coinPools = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (GameObject prefab in coinPrefabs)
        {
            if (prefab != null)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject coin = CreateObstacleInstance(prefab);
                    coin.SetActive(false);
                    objectQueue.Enqueue(coin);
                }

                coinPools[prefab] = objectQueue;
            }
        }

        powerupPools = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (GameObject prefab in powerupPrefabs)
        {
            if (prefab != null)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject powerup = CreateObstacleInstance(prefab);
                    powerup.SetActive(false);
                    objectQueue.Enqueue(powerup);
                }

                powerupPools[prefab] = objectQueue;
            }
        }
    }

    private GameObject CreateObstacleInstance(GameObject prefab)
    {
        GameObject obstacle = Instantiate(prefab);
        obstacle.name = prefab.name + "_Clone";

        // Add or get the ObstacleController component
        ObstacleController controller = obstacle.GetComponent<ObstacleController>();
        if (controller == null)
        {
            controller = obstacle.AddComponent<ObstacleController>();
        }
        controller.SetSpawner(this);

        return obstacle;
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Use current spawn intervals that change over time
            float spawnDelay = Random.Range(currentMinSpawnInterval, currentMaxSpawnInterval);
            yield return new WaitForSeconds(spawnDelay);
            Vector3 obstacleSpawn = SpawnRandomObstacle();
            if (obstacleSpawn != new Vector3(1000, 1000, 1000))
            {
                SpawnRandomCoin(obstacleSpawn);
                SpawnRandomPowerup(obstacleSpawn);
            }
        }
    }

    private Vector3 SpawnRandomObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned to the spawner!");
            return new Vector3(1000, 1000, 1000);
        }

        int randomIndex = Random.Range(0, obstaclePrefabs.Count);
        GameObject obstaclePrefab = obstaclePrefabs[randomIndex];

        if (obstaclePrefab == null)
        {
            Debug.LogWarning("One of the obstacle prefabs is null!");
            return new Vector3(1000, 1000, 1000);
        }

        // Get random spawn position from the fixed positions array
        Vector3 spawnPosition = GetRandomSpawnPosition();

        SpawnObstacle(obstaclePrefab, spawnPosition);
        return spawnPosition;
    }

    private void SpawnRandomCoin(Vector3 obstacleSpawn)
    {
        if (coinPrefabs == null || coinPrefabs.Count == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned to the spawner!");
            return;
        }

        int randomIndex = Random.Range(0, coinPrefabs.Count);
        GameObject coinPrefab = coinPrefabs[randomIndex];

        if (coinPrefab == null)
        {
            Debug.LogWarning("One of the obstacle prefabs is null!");
            return;
        }

        // Get random spawn position from the fixed positions array
        Vector3 spawnPosition = GetRandomSpawnPosition();
        while (spawnPosition == obstacleSpawn)
        {
            spawnPosition = GetRandomSpawnPosition();
        }

        StartCoroutine(SpawnCoin(coinPrefab, spawnPosition));
    }

    private void SpawnRandomPowerup(Vector3 obstacleSpawn)
    {
        if (powerupPrefabs == null || powerupPrefabs.Count == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned to the spawner!");
            return;
        }

        int randomIndex = Random.Range(0, powerupPrefabs.Count);
        GameObject powerupPrefab = powerupPrefabs[randomIndex];

        if (powerupPrefab == null)
        {
            Debug.LogWarning("One of the obstacle prefabs is null!");
            return;
        }

        // Get random spawn position from the fixed positions array
        Vector3 spawnPosition = GetRandomSpawnPosition();
        while (spawnPosition == obstacleSpawn)
        {
            spawnPosition = GetRandomSpawnPosition();
        }

        StartCoroutine(SpawnPowerup(powerupPrefab, spawnPosition));
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPositions == null || spawnPositions.Length == 0)
        {
            Debug.LogWarning("No spawn positions defined! Using default position.");
            return new Vector3(0f, 8f, 0f);
        }

        // Return a random position from the spawnPositions array
        return spawnPositions[Random.Range(0, spawnPositions.Length)];
    }

    private void SpawnObstacle(GameObject obstaclePrefab, Vector3 position)
    {
        GameObject obstacleInstance;

        if (useObjectPooling && obstaclePools != null && obstaclePools.ContainsKey(obstaclePrefab))
        {
            Queue<GameObject> pool = obstaclePools[obstaclePrefab];

            if (pool.Count > 0)
            {
                obstacleInstance = pool.Dequeue();
            }
            else
            {
                // Create new instance if pool is empty
                obstacleInstance = CreateObstacleInstance(obstaclePrefab);
            }

            obstacleInstance.transform.position = position;
            obstacleInstance.SetActive(true);

            // Initialize the obstacle
            ObstacleController controller = obstacleInstance.GetComponent<ObstacleController>();
            if (controller != null)
            {
                controller.Initialize();
            }
        }
        else
        {
            // Non-pooled spawning (creates new instance each time)
            obstacleInstance = CreateObstacleInstance(obstaclePrefab);
            obstacleInstance.transform.position = position;
            obstacleInstance.SetActive(true);
        }
    }

    private IEnumerator SpawnCoin(GameObject coinPrefab, Vector3 position)
    {
        int numCoins = Random.Range(1, maxCoinPerLane);
        while (numCoins > 0)
        {
            GameObject coinInstance;

            if (useObjectPooling && coinPools != null && coinPools.ContainsKey(coinPrefab))
            {
                Queue<GameObject> pool = coinPools[coinPrefab];

                if (pool.Count > 0)
                {
                    coinInstance = pool.Dequeue();
                }
                else
                {
                    // Create new instance if pool is empty
                    coinInstance = CreateObstacleInstance(coinPrefab);
                }

                coinInstance.transform.position = position;
                coinInstance.SetActive(true);

                // Initialize the obstacle
                ObstacleController controller = coinInstance.GetComponent<ObstacleController>();
                if (controller != null)
                {
                    controller.Initialize();
                }
            }
            else
            {
                // Non-pooled spawning (creates new instance each time)
                coinInstance = CreateObstacleInstance(coinPrefab);
                coinInstance.transform.position = position;
                coinInstance.SetActive(true);
            }
            yield return new WaitForSeconds(coinSpawnDelay);
            numCoins--;
        }
    }

    private IEnumerator SpawnPowerup(GameObject powerupPrefab, Vector3 position)
    {
        if (powerupCanSpawn)
        {
            powerupCanSpawn = false;

            GameObject powerupInstance;

            if (useObjectPooling && powerupPools != null && powerupPools.ContainsKey(powerupPrefab))
            {
                Queue<GameObject> pool = powerupPools[powerupPrefab];

                if (pool.Count > 0)
                {
                    powerupInstance = pool.Dequeue();
                }
                else
                {
                    // Create new instance if pool is empty
                    powerupInstance = CreateObstacleInstance(powerupPrefab);
                }

                powerupInstance.transform.position = position;
                powerupInstance.SetActive(true);

                // Initialize the obstacle
                ObstacleController controller = powerupInstance.GetComponent<ObstacleController>();
                if (controller != null)
                {
                    controller.Initialize();
                }
            }
            else
            {
                // Non-pooled spawning (creates new instance each time)
                powerupInstance = CreateObstacleInstance(powerupPrefab);
                powerupInstance.transform.position = position;
                powerupInstance.SetActive(true);
            }
            yield return new WaitForSeconds(powerupSpawnDelay);
            powerupCanSpawn = true;
        }
    }

    public void ReturnObstacleToPool(GameObject obstacle, GameObject originalPrefab)
    {
        if (useObjectPooling && obstaclePools != null && obstaclePools.ContainsKey(originalPrefab))
        {
            obstacle.SetActive(false);
            obstaclePools[originalPrefab].Enqueue(obstacle);
        }
        else
        {
            // If not using pooling, destroy the clone but keep original prefab
            Destroy(obstacle);
        }
    }

    // Draw spawn positions in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if (spawnPositions != null)
        {
            foreach (Vector3 spawnPos in spawnPositions)
            {
                Gizmos.DrawWireSphere(spawnPos, 0.3f);
            }
        }
    }

    // Public methods to manage spawn positions at runtime
    public void AddSpawnPosition(Vector3 newPosition)
    {
        List<Vector3> positionsList = new List<Vector3>(spawnPositions);
        positionsList.Add(newPosition);
        spawnPositions = positionsList.ToArray();
    }

    public void RemoveSpawnPosition(Vector3 positionToRemove)
    {
        List<Vector3> positionsList = new List<Vector3>(spawnPositions);
        positionsList.Remove(positionToRemove);
        spawnPositions = positionsList.ToArray();
    }

    public void ClearAllSpawnPositions()
    {
        spawnPositions = new Vector3[0];
    }
}
