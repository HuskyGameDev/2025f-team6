using UnityEngine;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource soundFXObject; 

    private void Awake() 
    {
        instance ??= this;
    }


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = UnityEngine.Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }


    public void PlayStartLoopStop(AudioClip startClip, AudioClip loopClip, AudioClip endClip, Transform spawnTransform, float volume, Func<bool> isHolding)
    {
        StartCoroutine(ExecuteStartLoopStop(startClip, loopClip, endClip, spawnTransform, volume, isHolding));
    }

    private IEnumerator ExecuteStartLoopStop(AudioClip start, AudioClip loop, AudioClip end, Transform spawn, float volume, Func<bool> isHolding)
    {
        // 1. Instantiate audio object
        AudioSource audioSource = Instantiate(soundFXObject, spawn.position, Quaternion.identity);
        audioSource.volume = volume;

        // 2. Play Start
        audioSource.clip = start;
        audioSource.Play();

        // Wait for start to finish OR for the player to release early
        float startTimer = 0;
        while (startTimer < start.length && isHolding())
        {
            startTimer += Time.deltaTime;
            yield return null;
        }

        // 3. Play Loop (if still holding)
        if (isHolding())
        {
            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.Play();

            // Wait for release
            while (isHolding())
            {
                // Update position if the spawn transform is moving
                if (spawn != null) audioSource.transform.position = spawn.position;
                yield return null;
            }
        }

        // 4. Play End
        audioSource.loop = false;
        audioSource.clip = end;
        audioSource.Play();

        // 5. Cleanup
        Destroy(audioSource.gameObject, end.length);
    }

}


