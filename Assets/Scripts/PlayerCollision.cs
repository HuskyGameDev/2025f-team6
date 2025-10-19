using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    [Header("Collision Effects")]
    [SerializeField] private ParticleSystem collisionParticle;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Player Effects")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private PlayerControl pc;

    private AudioSource audioSource;
    private Color originalColor;
    private Vector3 shakeStartPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pc = GetComponent<PlayerControl>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleCollision(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleCollision(collision.gameObject);
        }
    }

    public void HandleCollision(GameObject obstacle)
    {
        // Store current position for screen shake (where player actually is when hit)
        shakeStartPosition = transform.position;

        // Play particle effect
        if (collisionParticle != null)
        {
            Instantiate(collisionParticle, transform.position, Quaternion.identity);
        }

        // Play sound
        if (collisionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }

        // Screen shake
        StartCoroutine(ScreenShake());

        // Player flash
        StartCoroutine(PlayerFlash());

        // Note: The obstacle will handle its own deactivation through its ObstacleController
        // We don't destroy it here anymore

        // Add your game logic here (reduce health, game over, etc.)
        OnPlayerHit();
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = shakeStartPosition.x + Random.Range(-1f, 1f) * shakeMagnitude;
            float y = shakeStartPosition.y + Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(x, y, shakeStartPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return to the position where the player was when hit, NOT the original spawn position
        pc.RestartMotion();
    }

    private IEnumerator PlayerFlash()
    {
        if (playerSprite != null)
        {
            playerSprite.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = originalColor;
        }
    }

    protected virtual void OnPlayerHit()
    {
        Debug.Log("Player hit by obstacle!");
        // Add your game-specific logic here
    }
}