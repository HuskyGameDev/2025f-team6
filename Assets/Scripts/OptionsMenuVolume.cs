using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using System.Collections;

public class OptionsMenuVolume : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public AudioSource sfxPreviewSound; // assign in inspector

    const string MasterKey = "Master";
    const string MusicKey = "Music";
    const string SfxKey = "SFX";

    private bool isDragging = false;
    private Coroutine dragCoroutine;

    void Start()
    {
        if (!PlayerPrefs.HasKey(MasterKey))
        {
            PlayerPrefs.SetFloat(MasterKey, 1f);
            PlayerPrefs.SetFloat(MusicKey, 1f);
            PlayerPrefs.SetFloat(SfxKey, 1f);
            PlayerPrefs.Save();
        }

        masterSlider.value = PlayerPrefs.GetFloat(MasterKey);
        musicSlider.value = PlayerPrefs.GetFloat(MusicKey);
        sfxSlider.value = PlayerPrefs.GetFloat(SfxKey);

        ApplyMaster(masterSlider.value);
        ApplyMusic(musicSlider.value);
        ApplySfx(sfxSlider.value);

        masterSlider.onValueChanged.AddListener(ApplyMaster);
        musicSlider.onValueChanged.AddListener(ApplyMusic);
        sfxSlider.onValueChanged.AddListener(ApplySfx);
    }

    void SetMixerVolume(string exposedParam, float linearValue)
    {
        if (linearValue <= 0f)
            mixer.SetFloat(exposedParam, -80f);
        else
            mixer.SetFloat(exposedParam, Mathf.Log10(linearValue) * 20f);
    }

    public void ApplyMaster(float value)
    {
        SetMixerVolume("MasterVolume", value);
        PlayerPrefs.SetFloat(MasterKey, value);

        if (!isDragging)
        {
            PlayPreviewSound();
        }
    }

    public void ApplyMusic(float value)
    {
        SetMixerVolume("MusicVolume", value);
        PlayerPrefs.SetFloat(MusicKey, value);

        if (!isDragging)
        {
            PlayPreviewSound();
        }
    }

    public void ApplySfx(float value)
    {
        SetMixerVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SfxKey, value);

        if (!isDragging)
        {
            PlayPreviewSound();
        }
    }

    // ---------------- DRAG LOGIC ----------------

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            isDragging = true;

            if (dragCoroutine != null)
                StopCoroutine(dragCoroutine);

            dragCoroutine = StartCoroutine(DelayedPreview());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragCoroutine != null)
            StopCoroutine(dragCoroutine);

        if (isDragging)
        {
            PlayPreviewSound(); // play immediately after release
        }

        isDragging = false;
    }

    IEnumerator DelayedPreview()
    {
        yield return new WaitForSeconds(2f);

        if (isDragging)
        {
            PlayPreviewSound();
        }
    }

    void PlayPreviewSound()
    {
        if (sfxPreviewSound != null)
            sfxPreviewSound.Play();
    }
}
