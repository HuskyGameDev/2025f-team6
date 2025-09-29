using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Spawner_Script : MonoBehaviour
{
    [System.Serializable]
    public class Lane
    {
        public Transform spawnPoint;
        public bool isActive = true;
    }

    [Header("Spawner Settings")]
    public List<Lane> lanes = new List<Lane>();
    public List<GameObject> obstaclePrefabs = new List<GameObject>();

    [Header("Timing Settings")]
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 4f;

    [Header("Debug")]
    public bool debugMode = false;

    private void Start()
    {
        // Validate setup
        if (lanes.Count == 0 || obstaclePrefabs.Count == 0)
        {
            Debug.LogError("ObstacleSpawner: Please assign lanes and obstacle prefabs!");
            return;
        }

        // Start spawning coroutine
        StartCoroutine(SpawnObstaclesRoutine());
    }

    private IEnumerator SpawnObstaclesRoutine()
    {
        while (true)
        {
            // Wait for random interval
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Spawn obstacle
            SpawnRandomObstacle();
        }
    }

    private void SpawnRandomObstacle()
    {
        // Get random lane
        List<Lane> activeLanes = lanes.FindAll(lane => lane.isActive);
        if (activeLanes.Count == 0) return;

        Lane randomLane = activeLanes[Random.Range(0, activeLanes.Count)];

        // Get random obstacle
        GameObject randomObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

        // Spawn obstacle
        if (randomObstacle != null && randomLane.spawnPoint != null)
        {
            Instantiate(randomObstacle, randomLane.spawnPoint.position, randomLane.spawnPoint.rotation);

            if (debugMode)
            {
                Debug.Log($"Spawned {randomObstacle.name} in lane {lanes.IndexOf(randomLane)}");
            }
        }
    }

    // Public methods to control lanes
    public void SetLaneActive(int laneIndex, bool active)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
        {
            lanes[laneIndex].isActive = active;
        }
    }

    public void ToggleLane(int laneIndex)
    {
        if (laneIndex >= 0 && laneIndex < lanes.Count)
        {
            lanes[laneIndex].isActive = !lanes[laneIndex].isActive;
        }
    }
}