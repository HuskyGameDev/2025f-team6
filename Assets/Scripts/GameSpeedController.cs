//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class GameSpeedController : MonoBehaviour
//{
//    public static GameSpeedController Instance;

//    [Header("Base Speeds")]
//    public float easyStartSpeed = 3f;
//    public float normalStartSpeed = 5f;
//    public float hardStartSpeed = 7f;

//    [Header("Progression")]
//    public float speedIncreasePerSecond = 0.25f;

//    public float CurrentSpeed { get; private set; }

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        DontDestroyOnLoad(gameObject);

//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }
//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        // Reset only when entering gameplay scene
//        if (scene.name == "Main")
//        {
//            ResetSpeed();
//        }
//    }

//    private void Start()
//    {
//        CurrentSpeed = GetStartSpeedFromDifficulty();
//    }

//    private float debugTimer;

//    private void Update()
//    {
//        CurrentSpeed += speedIncreasePerSecond * Time.deltaTime;

//        debugTimer += Time.deltaTime;
//        if (debugTimer >= 1f)
//        {
//            debugTimer = 0f;

//            Debug.Log(
//                $"[GameSpeedController] Difficulty = {DifficultyButtonManager.difficultyValue} | Speed = {CurrentSpeed:F2}"
//            );
//        }
//    }

//    private float GetStartSpeedFromDifficulty()
//    {
//        switch (DifficultyButtonManager.difficultyValue)
//        {
//            case 1: return easyStartSpeed;
//            case 3: return hardStartSpeed;
//            default: return normalStartSpeed;  // fix, default's currently ez
//        }
//    }

//    public void ResetSpeed()
//    {
//        CurrentSpeed = GetStartSpeedFromDifficulty();

//        Debug.Log(
//            $"[GameSpeedController] RESET | Difficulty = {DifficultyButtonManager.difficultyValue} | StartSpeed = {CurrentSpeed}"
//        );
//    }

//    // I am too tired of this error
//    public static GameSpeedController GetOrCreate()
//    {
//        if (Instance == null)
//        {
//            GameObject go = new GameObject("GameSpeedController");
//            Instance = go.AddComponent<GameSpeedController>();
//        }
//        return Instance;
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSpeedController : MonoBehaviour
{
    public static GameSpeedController Instance;

    [Header("Base Speeds")]
    public float easyStartSpeed = 3f;
    public float normalStartSpeed = 5f;
    public float hardStartSpeed = 7f;

    [Header("Progression Rates")]
    public float easyIncrease = 0.0625f;
    public float normalIncrease = 0.125f;
    public float hardIncrease = 0.25f;

    [Header("Limits")]
    public float maxSpeed = 15f;

    public float CurrentSpeed { get; private set; }

    private float speedIncreasePerSecond;
    private float debugTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        InitializeSpeed();
    }

    private void Update()
    {
        CurrentSpeed += speedIncreasePerSecond * Time.deltaTime;

        // Cap the speed
        if (CurrentSpeed > maxSpeed)
            CurrentSpeed = maxSpeed;

        // Debug print once per second
        debugTimer += Time.deltaTime;
        if (debugTimer >= 1f)
        {
            debugTimer = 0f;
            Debug.Log($"[GameSpeedController] Difficulty = {DifficultyButtonManager.difficultyValue} | Speed = {CurrentSpeed:F2}");
        }
    }

    private float GetStartSpeedFromDifficulty()
    {
        switch (DifficultyButtonManager.difficultyValue)
        {
            case 2: return normalStartSpeed;
            case 3: return hardStartSpeed;
            default: return easyStartSpeed; // default = easy
        }
    }

    private float GetIncreaseRateFromDifficulty()
    {
        switch (DifficultyButtonManager.difficultyValue)
        {
            case 2: return normalIncrease;
            case 3: return hardIncrease;
            default: return easyIncrease;
        }
    }

    public void ResetSpeed()
    {
        InitializeSpeed();
        Debug.Log($"[GameSpeedController] RESET | Difficulty = {DifficultyButtonManager.difficultyValue} | StartSpeed = {CurrentSpeed}");
    }

    private void InitializeSpeed()
    {
        CurrentSpeed = GetStartSpeedFromDifficulty();
        speedIncreasePerSecond = GetIncreaseRateFromDifficulty();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            ResetSpeed();
        }
    }

    // Safety getter if obstacles or background need it before Awake/Start
    public static GameSpeedController GetOrCreate()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("GameSpeedController");
            Instance = go.AddComponent<GameSpeedController>();
        }
        return Instance;
    }
}
