using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useRandomSpeed = false;
    [SerializeField] private float minSpeed = 3f;
    [SerializeField] private float maxSpeed = 8f;

    [Header("Destruction Settings")]
    [SerializeField] private float destroyYPosition = -10f;
    [SerializeField] private bool destroyWhenInvisible = true;

    private Camera mainCamera;
    private float speed;
    private ObstacleSpawner spawner;
    private GameObject originalPrefab;
    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        // Move obstacle downward
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Check if off screen
        if (IsOffScreen())
        {
            DeactivateObstacle();
        }
    }

    public void SetSpawner(ObstacleSpawner obstacleSpawner)
    {
        this.spawner = obstacleSpawner;
    }

    public void Initialize()
    {
        mainCamera = Camera.main;

        // Set movement speed
        if (useRandomSpeed)
        {
            speed = Random.Range(minSpeed, maxSpeed);
        }
        else
        {
            speed = moveSpeed;
        }

        isActive = true;

        // Store reference to original prefab for pooling
        if (originalPrefab == null)
        {
            string originalName = gameObject.name.Replace("_Clone", "").Replace("(Clone)", "");
            originalPrefab = Resources.Load<GameObject>(originalName);
        }
    }

    private bool IsOffScreen()
    {
        if (destroyWhenInvisible && mainCamera != null)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            return screenPoint.y < -0.1f; // Slightly below the screen
        }
        else
        {
            return transform.position.y < destroyYPosition;
        }
    }

    private void DeactivateObstacle()
    {
        isActive = false;

        if (spawner != null && originalPrefab != null)
        {
            // Return to pool instead of destroying
            spawner.ReturnObstacleToPool(gameObject, originalPrefab);
        }
        else
        {
            // Fallback: just deactivate
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
    }

    private void HandlePlayerCollision(GameObject player)
    {
        // Trigger collision effects on player
        PlayerCollision playerCollision = player.GetComponent<PlayerCollision>();
        if (playerCollision != null)
        {
            // You might want to pass this obstacle to the player collision handler
            playerCollision.HandleCollision(gameObject);
        }

        // Deactivate this obstacle after collision
        DeactivateObstacle();
    }

    // Optional: Visualize destruction boundary in editor
    private void OnDrawGizmosSelected()
    {
        if (!destroyWhenInvisible)
        {
            Gizmos.color = Color.yellow;
            Vector3 destructionPoint = new Vector3(transform.position.x, destroyYPosition, transform.position.z);
            Gizmos.DrawLine(transform.position, destructionPoint);
            Gizmos.DrawWireCube(destructionPoint, Vector3.one);
        }
    }
}