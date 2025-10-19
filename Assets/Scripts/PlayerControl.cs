using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float speedCurve = 0.5f; // Needs to be >0 and <2
    [SerializeField] private float yPos = -3.5f;

    [Header("Player Positions")]
    [SerializeField]
    private Vector3[] playerPositions = new Vector3[]
    {
        new Vector3(-3f, -3.5f, 0f),
        new Vector3(-1f, -3.5f, 0f),
        new Vector3(1f, -3.5f, 0f),
        new Vector3(3f, -3.5f, 0f)
    };

    [Header("Hit Effect Settings")]
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private AudioClip hitSound;

    private int currentPosition;
    private float interpolator;
    private Vector3 oldPosition;
    private float posDiff;
    private bool isMoving = false;

    // Rendering components
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Color originalColor;
    private bool isBlinking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Store original color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Start player in center position (position 1 or 2)
        int startingPosition = 1;
        Vector3 startPos = playerPositions[startingPosition];
        transform.SetPositionAndRotation(startPos, Quaternion.identity);
        oldPosition = transform.position;
        currentPosition = startingPosition;
        interpolator = 0.0f;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the input from the player to see if they are moving between positions
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentPosition > 0)
            {
                StartMovementToPosition(currentPosition - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentPosition < playerPositions.Length - 1)
            {
                StartMovementToPosition(currentPosition + 1);
            }
        }

        // Only update movement if we're actively moving between positions
        if (isMoving)
        {
            MovePlayer();
        }
    }

    private void StartMovementToPosition(int newPosition)
    {
        currentPosition = newPosition;
        interpolator = 0f;
        oldPosition = transform.position;

        // Calculate the absolute distance to the target position
        posDiff = Mathf.Abs(oldPosition.x - playerPositions[currentPosition].x);

        // Ensure posDiff is not zero to avoid division by zero
        if (posDiff < 0.001f)
        {
            posDiff = 0.001f;
        }

        isMoving = true;
    }

    private void MovePlayer()
    {
        Vector3 targetPosition = playerPositions[currentPosition];

        // Safe Lerp calculation
        float lerpValue = Mathf.Clamp01(interpolator);
        float newX = Mathf.Lerp(oldPosition.x, targetPosition.x, lerpValue);

        // Ensure we don't get NaN values
        if (float.IsNaN(newX))
        {
            newX = transform.position.x;
        }

        transform.position = new Vector3(newX, targetPosition.y, targetPosition.z);

        // Check if we've reached the target position
        float currentDistance = Mathf.Abs(transform.position.x - targetPosition.x);

        // Safe calculation for movement curve
        if (posDiff > 0.001f && currentDistance > 0.001f)
        {
            float distanceRatio = currentDistance / posDiff;
            float diffPercent = 1 - speedCurve * Mathf.Abs(distanceRatio - 0.5f);

            // Ensure diffPercent is valid
            if (!float.IsNaN(diffPercent) && !float.IsInfinity(diffPercent))
            {
                interpolator += Mathf.Clamp(diffPercent, 0.1f, 2f) * speed * Time.deltaTime;
            }
            else
            {
                interpolator += speed * Time.deltaTime;
            }
        }
        else
        {
            interpolator += speed * Time.deltaTime;
        }

        // Check if movement is complete
        if (interpolator >= 1f || currentDistance < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
            interpolator = 0f;
        }
    }

    // Called when player collides with an obstacle
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            OnHit();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            OnHit();
        }
    }

    public void OnHit()
    {
        // Start blink effect
        if (!isBlinking)
        {
            StartCoroutine(BlinkEffect());
        }

        // Play hit sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Play particle effect
        if (hitParticles != null)
        {
            Instantiate(hitParticles, transform.position, Quaternion.identity);
        }

        // Add your game logic here (reduce health, etc.)
        Debug.Log("Player hit!");
    }

    private IEnumerator BlinkEffect()
    {
        isBlinking = true;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            if (spriteRenderer != null)
            {
                // Toggle between hit color and original color
                spriteRenderer.color = (spriteRenderer.color == originalColor) ? hitColor : originalColor;
            }

            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure we end with the original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isBlinking = false;
    }

    // Public method to trigger hit effect from other scripts
    public void TriggerHitEffect()
    {
        OnHit();
    }

    // Public methods to manage positions at runtime
    public void AddPlayerPosition(Vector3 newPosition)
    {
        System.Collections.Generic.List<Vector3> positionsList = new System.Collections.Generic.List<Vector3>(playerPositions);
        positionsList.Add(newPosition);
        playerPositions = positionsList.ToArray();
    }

    public void RemovePlayerPosition(int index)
    {
        if (index >= 0 && index < playerPositions.Length)
        {
            System.Collections.Generic.List<Vector3> positionsList = new System.Collections.Generic.List<Vector3>(playerPositions);
            positionsList.RemoveAt(index);
            playerPositions = positionsList.ToArray();

            // Adjust current position if needed
            if (currentPosition >= playerPositions.Length)
            {
                currentPosition = playerPositions.Length - 1;
                transform.position = playerPositions[currentPosition];
            }
        }
    }

    public Vector3 GetCurrentTargetPosition()
    {
        return playerPositions[currentPosition];
    }

    public int GetCurrentPositionIndex()
    {
        return currentPosition;
    }

    // Draw player positions in editor
    private void OnDrawGizmosSelected()
    {
        if (playerPositions != null)
        {
            Gizmos.color = Color.blue;
            foreach (Vector3 pos in playerPositions)
            {
                Gizmos.DrawWireCube(pos, new Vector3(0.5f, 1f, 0.5f));
            }

            // Highlight current target position
            if (currentPosition >= 0 && currentPosition < playerPositions.Length)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(playerPositions[currentPosition], new Vector3(0.7f, 1.2f, 0.7f));

                // Draw line from player to target if moving
                if (isMoving)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, playerPositions[currentPosition]);
                }
            }
        }
    }
}