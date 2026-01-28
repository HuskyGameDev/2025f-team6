using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private bool useRandomSpeed = false;

    [SerializeField]
    private float minSpeed = 3f;

    [SerializeField]
    private float maxSpeed = 8f;

    [SerializeField]
    private float verticalSpeedMultiplier = 1f;

    [Header("Destruction Settings")]
    [SerializeField]
    private float destroyYPosition = -10f;

    [SerializeField]
    private bool destroyWhenInvisible = true;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip hitSoundClip;

    private Camera mainCamera;

    [SerializeField]
    private float speed;
    private ObstacleSpawner spawner;
    private GameObject originalPrefab;
    public bool isActive = false;

    // Speed multiplier from spawner
    private float speedMultiplier = 1f;
    private float baseSpeed; // Store the original speed

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        float scrollSpeed = speed;

        if (ScrollSpeedProvider.Instance != null)
        {
            scrollSpeed = ScrollSpeedProvider.Instance.CurrentSpeed;
        }

        transform.Translate(Vector3.down * scrollSpeed * verticalSpeedMultiplier * Time.deltaTime);

        // Check if off screen
        if (IsOffScreen())
        {
            Destroy(this.gameObject);
        }
    }

    public void SetSpawner(ObstacleSpawner obstacleSpawner)
    {
        this.spawner = obstacleSpawner;

        // Subscribe to speed multiplier changes
        if (spawner != null)
        {
            spawner.OnSpeedMultiplierChanged += OnSpeedMultiplierChanged;
        }
    }

    public void Initialize()
    {
        mainCamera = Camera.main;

        float scrollSpeed = moveSpeed;

        if (ScrollSpeedProvider.Instance != null)
        {
            scrollSpeed = ScrollSpeedProvider.Instance.CurrentSpeed;
        }

        speed = scrollSpeed;

        if (useRandomSpeed && ScrollSpeedProvider.Instance != null)
        {
            float baseScroll = ScrollSpeedProvider.Instance.GetBaseScrollSpeed();
            if (baseScroll > 0.0001f)
            {
                float randomAbsolute = Random.Range(minSpeed, maxSpeed);
                verticalSpeedMultiplier = randomAbsolute / baseScroll;
            }
        }

        baseSpeed = speed;

        // Get current speed multiplier from spawner if available
        if (spawner != null)
        {
            speedMultiplier = spawner.GetCurrentSpeedMultiplier();
        }

        isActive = true;

        // Store reference to original prefab for pooling
        if (originalPrefab == null)
        {
            string originalName = gameObject.name.Replace("_Clone", "").Replace("(Clone)", "");
            originalPrefab = Resources.Load<GameObject>(originalName);
        }
    }

    private void OnSpeedMultiplierChanged(float newMultiplier)
    {
        speedMultiplier = newMultiplier;
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

    private void HandlePlayerCollision(GameObject player)
    {
        // Trigger collision effects on player
        PlayerCollision playerCollision = player.GetComponent<PlayerCollision>();

        if (playerCollision != null)
        {
            // Play SFX
            AudioManager.instance.PlaySoundFXClip(hitSoundClip, transform, 1f);

            // You might want to pass this obstacle to the player collision handler
            playerCollision.HandleCollision(gameObject);
        }

        // Deactivate this obstacle after collision
        DeactivateObstacle();
    }

    // Make sure to unsubscribe from events
    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnSpeedMultiplierChanged -= OnSpeedMultiplierChanged;
        }
    }

    // Optional: Visualize destruction boundary in editor
    private void OnDrawGizmosSelected()
    {
        if (!destroyWhenInvisible)
        {
            Gizmos.color = Color.yellow;
            Vector3 destructionPoint = new Vector3(
                transform.position.x,
                destroyYPosition,
                transform.position.z
            );
            Gizmos.DrawLine(transform.position, destructionPoint);
            Gizmos.DrawWireCube(destructionPoint, Vector3.one);
        }
    }
}
