using UnityEngine;

public class ScrollSpeedProvider : MonoBehaviour
{
    [SerializeField]
    private float baseScrollSpeed = 1f;

    [SerializeField]
    private float difficultyMultiplier = 1f;

    public static ScrollSpeedProvider Instance { get; private set; }

    public float CurrentSpeed
    {
        get { return baseScrollSpeed * difficultyMultiplier; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetBaseScrollSpeed(float speed)
    {
        baseScrollSpeed = speed;
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        difficultyMultiplier = multiplier;
    }

    public float GetDifficultyMultiplier()
    {
        return difficultyMultiplier;
    }

    public float GetBaseScrollSpeed()
    {
        return baseScrollSpeed;
    }
}
