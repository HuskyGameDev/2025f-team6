using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 5f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Spawn Positions")]
    [SerializeField]
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-2f, 8f, 0f),
        new Vector3(0f, 8f, 0f),
        new Vector3(2f, 8f, 0f),
        new Vector3(4f, 8f, 0f)    
    };

    [Header("Obstacle Pooling (Optional)")]
    [SerializeField] private bool useObjectPooling = true;
    [SerializeField] private int poolSize = 10;

    private Coroutine spawnCoroutine;
    private Dictionary<GameObject, Queue<GameObject>> obstaclePools;

    void Start()
    {
        if (useObjectPooling)
        {
            InitializePools();
        }

        if (spawnOnStart)
        {
            StartSpawning();
        }
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
            float spawnDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnDelay);
            SpawnRandomObstacle();
        }
    }

    private void SpawnRandomObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0)
        {
            Debug.LogWarning("No obstacle prefabs assigned to the spawner!");
            return;
        }

        int randomIndex = Random.Range(0, obstaclePrefabs.Count);
        GameObject obstaclePrefab = obstaclePrefabs[randomIndex];

        if (obstaclePrefab == null)
        {
            Debug.LogWarning("One of the obstacle prefabs is null!");
            return;
        }

        // Get random spawn position from the fixed positions array
        Vector3 spawnPosition = GetRandomSpawnPosition();

        SpawnObstacle(obstaclePrefab, spawnPosition);
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