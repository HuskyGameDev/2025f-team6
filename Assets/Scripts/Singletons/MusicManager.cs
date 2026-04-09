using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public static bool isMenuMusicPlaying = false;
    public static bool isInGameMusicPlaying = false;

    [SerializeField] private AudioSource musicFXObject;

    [Header("MainMenu Music")]
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private List<string> mainMusicScenes;

    [Header("In Game Music")]
    [SerializeField] private AudioClip inGameMusic;
    [SerializeField] private string mainSceneName = "Main";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        HandleSceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneMusic(scene.name);
    }

    private void HandleSceneMusic(string sceneName)
    {
        if (sceneName == mainSceneName)
        {
            PlayMusic(inGameMusic, transform, 1f, new List<string> { mainSceneName }, true);
        }
        else if (mainMusicScenes.Contains(sceneName))
        {
            PlayMusic(mainMusic, transform, 1f, mainMusicScenes, false);
        }
    }

    public void PlayMusic(AudioClip audioClip, Transform spawnTransform, float volume, List<string> allowedScenes, bool isInGame)
    {
        if (audioClip == null || musicFXObject == null) return;

        if (isInGame)
        {
            if (isInGameMusicPlaying) return;
            isInGameMusicPlaying = true;
        }
        else
        {
            if (isMenuMusicPlaying) return;
            isMenuMusicPlaying = true;
        }

        AudioSource audioSource = Instantiate(musicFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        DontDestroyOnLoad(audioSource.gameObject);

        MusicDestructor destructor = audioSource.gameObject.AddComponent<MusicDestructor>();
        destructor.allowedScenes = allowedScenes;
        destructor.isInGameMusic = isInGame;
    }
}

public class MusicDestructor : MonoBehaviour
{
    public List<string> allowedScenes;
    public bool isInGameMusic;

    void OnEnable()
    {
        SceneManager.sceneLoaded += CheckScene;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckScene;
    }

    void CheckScene(Scene scene, LoadSceneMode mode)
    {
        if (allowedScenes == null || !allowedScenes.Contains(scene.name))
        {
            if (isInGameMusic)
                MusicManager.isInGameMusicPlaying = false;
            else
                MusicManager.isMenuMusicPlaying = false;

            Destroy(gameObject);
        }
    }
}