using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private float immunityDuration = 2.0f;
    [SerializeField] private BoxCollider2D pHitbox;
    [SerializeField] private PlayerControl pc;
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int lives = 3;
    [SerializeField] private string endScene;


    private GameObject heartA; // left-most
    private GameObject heartB;
    private GameObject heartC;

    private AudioSource audioSource;
    private Color originalColor;
    private Vector3 shakeStartPosition;

    private Boolean immunity = false;

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

        // find the hearts
        GameObject[] heartArr = GameObject.FindGameObjectsWithTag("Hearts");
        foreach (GameObject heart in heartArr)
        {
            if (heart.name == "HeartA")
                heartA = heart;
            else if (heart.name == "HeartB")
                heartB = heart;
            else if (heart.name == "HeartC")
                heartC = heart;
        }

        PlayerControl.coinsCollected = 0;
    }


    public void HandleCollision(GameObject obstacle)
    {   
        if (obstacle.CompareTag("Obstacle") && !immunity) {
            //Reduce Lives by 1
            lives--;
            BoxCollider2D obHitbox = obstacle.GetComponent<BoxCollider2D>();
            obHitbox.enabled = false;
            if(lives == 0)
            {
                UIController.setFinalScore(UIController.getFinalScore());
                SceneManager.LoadScene(endScene);
                //Pause the game (put all speeds to 0)
                //Pop up end screen
            }

            // Store current position for screen shake (where player actually is when hit)
            //shakeStartPosition = transform.position;

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
            //StartCoroutine(ScreenShake());

            // Player flash
            StartCoroutine(PlayerFlash());

            // Invicibility Frames
            StartCoroutine(iFrames());

            // Note: The obstacle will handle its own deactivation through its ObstacleController
            // We don't destroy it here anymore

            // Add your game logic here (reduce health, game over, etc.)
            OnPlayerHit();
            pc.TriggerHitEffect();
        }
        else if (obstacle.CompareTag("Coin")) {
            pc.CollectCoin();
        }
        else if (obstacle.CompareTag("Powerup"))
        {
            pc.GainPowerup(obstacle);
        }
        
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = shakeStartPosition.x + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = shakeStartPosition.y + UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

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

    private IEnumerator iFrames()
    {
        if (pHitbox != null)
        {
            immunity = true;
            yield return new WaitForSeconds(immunityDuration);
            immunity = false;
        }
    }

    protected virtual void OnPlayerHit()
    {
        Debug.Log("Player hit by obstacle! " + lives + " lives remain.");

        updateHearts(lives, true);
    }

    public void incrementLives()
    {
        if (lives < maxLives) {
            lives++;
        }
    }

    public int getLivesRemaining()
    {
        return lives;
    }

    public void updateHearts(int lives, bool lose)
    {
        if (heartA == null || heartB == null || heartC == null)
        {
            Debug.LogWarning("Heart sprite(s) not found.");
            return;
        }

        GameObject[] hearts = {null, heartA, heartB, heartC};
        
        if (lose)
        {
            GameObject heart = hearts[lives+1];
            heart.SetActive(false);
        }
        else
        {
            GameObject heart = hearts[lives];
            heart.SetActive(true);
        }

        // if (lives == 3)
        // {
        //     heart
        // }
        // // Hide based on remaining lives (called from PlayerCollision)
        // if (lives == 2) // first hit
        // {
        //     heartC.SetActive(!heartC.activeSelf);
        // }
        // if (lives == 1) // second hit
        // {
        //     heartB.SetActive(!heartB.activeSelf);
        // }
        // if (lives <= 0) // perish
        // {
        //     heartA.SetActive(!heartA.activeSelf);
        // }

        //     // Ensure the others are also hidden if lives <= 0
        //     //if (heartB != null) heartB.SetActive(false);
        //     //if (heartC != null) heartC.SetActive(false);
    }

    public void showAllHearts()
    {
        if (heartA == null || heartB == null || heartC == null)
        {
            Debug.LogWarning("Heart sprite(s) not found.");
            return;
        }

        // Show all hearts
        if (heartA != null) heartA.SetActive(true);
        if (heartB != null) heartB.SetActive(true);
        if (heartC != null) heartC.SetActive(true);
    }

    public bool checkImmunity()
    {
        return immunity;
    }
    public void toggleImmunity()
    {
        immunity = !immunity;
    }
}