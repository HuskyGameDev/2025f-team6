using UnityEngine;

public class Obstacle_Script : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Vector3 moveDirection = Vector3.down;

    [Header("Destruction")]
    public float destroyYPosition = -10f;

    private void Update()
    {
        // Move obstacle downward
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Destroy when off screen
        if (transform.position.y < destroyYPosition)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle player collision
        if (collision.CompareTag("Player"))
        {
            // Add your collision logic here
            Debug.Log("Player hit obstacle!");

            // Example: Destroy obstacle on collision
            Destroy(gameObject);
        }
    }
}