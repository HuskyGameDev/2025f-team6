using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public static bool isMenuMusicPlaying = false;

    [SerializeField] private AudioSource musicFXObject; 

    [Header("MainMenu Music")]
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private List<string> mainMusicScenes;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        PlayMusic(mainMusic, transform, 1f, mainMusicScenes);

    }


    public void PlayMusic(AudioClip audioClip, Transform spawnTransform, float volume, List<string> allowedScenes)
    {

        if (isMenuMusicPlaying) return;

        isMenuMusicPlaying = true;

        AudioSource audioSource = Instantiate(musicFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();

        DontDestroyOnLoad(audioSource.gameObject);

        MusicDestructor destructor = audioSource.gameObject.AddComponent<MusicDestructor>();
        destructor.allowedScenes = allowedScenes;
    }

}

// Helper class to handle auto-destruction
public class MusicDestructor : MonoBehaviour
{
    public List<string> allowedScenes;

    void OnEnable() => SceneManager.sceneLoaded += CheckScene;
    void OnDisable() => SceneManager.sceneLoaded -= CheckScene;

    void CheckScene(Scene scene, LoadSceneMode mode)
    {
        // If the new scene is NOT in our allowed list, destroy this music object
        if (!allowedScenes.Contains(scene.name))
        {
            MusicManager.isMenuMusicPlaying = false;
            Destroy(gameObject);
        }
    }
}


