using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        ApplySavedVolumes(); // Apply saved volumes at game start
    }

    public void SetVolume(string exposedParam, float linearValue)
    {
        if (linearValue <= 0f)
            mixer.SetFloat(exposedParam, -80f);
        else
            mixer.SetFloat(exposedParam, Mathf.Log10(linearValue) * 20f);
    }

    public void ApplySavedVolumes()
    {
        float master = PlayerPrefs.GetFloat("Master", 0.5f);
        float music = PlayerPrefs.GetFloat("Music", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFX", 0.5f);

        SetVolume("MasterVolume", master);
        SetVolume("MusicVolume", music);
        SetVolume("SFXVolume", sfx);
    }


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        int rand = UnityEngine.Random.Range(0, audioClips.Length);
        PlaySoundFXClip(audioClips[rand], spawnTransform, volume);
    }

    public void PlayStartLoopStop(AudioClip startClip, AudioClip loopClip, AudioClip endClip, Transform spawnTransform, float volume, Func<bool> isHolding)
    {
        StartCoroutine(ExecuteStartLoopStop(startClip, loopClip, endClip, spawnTransform, volume, isHolding));
    }

    private IEnumerator ExecuteStartLoopStop(AudioClip start, AudioClip loop, AudioClip end, Transform spawn, float volume, Func<bool> isHolding)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
        audioSource.volume = volume;

        // Play start
        audioSource.clip = start;
        audioSource.Play();

        float startTimer = 0f;
        while (startTimer < start.length && isHolding())
        {
            startTimer += Time.deltaTime;
            yield return null;
        }

        // Play loop if still holding
        if (isHolding())
        {   
            if (loop != null) {
                audioSource.clip = loop;
                audioSource.loop = true;
                audioSource.Play();
            }

            while (isHolding())
            {
                if (spawn != null) audioSource.transform.position = spawn.position;
                yield return null;
            }
        }

        if (audioSource != null)
        {
            // Play end
            audioSource.loop = false;
            audioSource.clip = end;
            audioSource.Play();

            Destroy(audioSource.gameObject, end.length);
        }
    }
}
