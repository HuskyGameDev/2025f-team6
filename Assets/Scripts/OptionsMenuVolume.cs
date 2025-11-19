//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Audio;

//public class OptionsMenuVolume : MonoBehaviour
//{
//    public AudioMixer mixer;
//    public Slider masterSlider;
//    public Slider musicSlider;
//    public Slider sfxSlider;

//    const string MasterKey = "Master";
//    const string MusicKey = "Music";
//    const string SfxKey = "SFX";

//    void Start()
//    {
//        // load saved linear [0..1] values
//        masterSlider.value = PlayerPrefs.GetFloat(MasterKey, 1f);
//        musicSlider.value = PlayerPrefs.GetFloat(MusicKey, 1f);
//        sfxSlider.value = PlayerPrefs.GetFloat(SfxKey, 1f);

//        // apply immediately
//        ApplyMaster(masterSlider.value);
//        ApplyMusic(musicSlider.value);
//        ApplySfx(sfxSlider.value);

//        // hook up listeners (or hook these methods in the slider's OnValueChanged in Inspector)
//        masterSlider.onValueChanged.AddListener(ApplyMaster);
//        musicSlider.onValueChanged.AddListener(ApplyMusic);
//        sfxSlider.onValueChanged.AddListener(ApplySfx);
//    }

//    // converts slider [0..1] to dB and sets exposed param
//    void SetMixerVolume(string exposedParam, float linearValue)
//    {
//        // avoid log(0). Unity treats -80 dB as silence
//        if (linearValue <= 0f)
//            mixer.SetFloat(exposedParam, -80f);
//        else
//            mixer.SetFloat(exposedParam, Mathf.Log10(linearValue) * 20f);
//    }

//    public void ApplyMaster(float value)
//    {
//        SetMixerVolume("MasterVolume", value);
//        PlayerPrefs.SetFloat(MasterKey, value);
//    }

//    public void ApplyMusic(float value)
//    {
//        SetMixerVolume("MusicVolume", value);
//        PlayerPrefs.SetFloat(MusicKey, value);
//    }

//    public void ApplySfx(float value)
//    {
//        SetMixerVolume("SFXVolume", value);
//        PlayerPrefs.SetFloat(SfxKey, value);
//    }
//}


// Version that sets sliders to be 0.5 instead of 0 at launch.
// If the sliders don't work or don't save in the build, use above version.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenuVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    const string MasterKey = "Master";
    const string MusicKey = "Music";
    const string SfxKey = "SFX";

    void Start()
    {
        // ---------- FIRST RUN DEFAULTS ----------
        if (!PlayerPrefs.HasKey(MasterKey))
        {
            PlayerPrefs.SetFloat(MasterKey, 0.5f);
            PlayerPrefs.SetFloat(MusicKey, 0.5f);
            PlayerPrefs.SetFloat(SfxKey, 0.5f);
            PlayerPrefs.Save();
        }

        // ---------- LOAD SAVED VALUES ----------
        masterSlider.value = PlayerPrefs.GetFloat(MasterKey);
        musicSlider.value = PlayerPrefs.GetFloat(MusicKey);
        sfxSlider.value = PlayerPrefs.GetFloat(SfxKey);

        // ---------- APPLY IMMEDIATELY ----------
        ApplyMaster(masterSlider.value);
        ApplyMusic(musicSlider.value);
        ApplySfx(sfxSlider.value);

        // ---------- HOOK LISTENERS ----------
        masterSlider.onValueChanged.AddListener(ApplyMaster);
        musicSlider.onValueChanged.AddListener(ApplyMusic);
        sfxSlider.onValueChanged.AddListener(ApplySfx);
    }

    // converts slider [0..1] to dB and sets exposed param
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
    }

    public void ApplyMusic(float value)
    {
        SetMixerVolume("MusicVolume", value);
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void ApplySfx(float value)
    {
        SetMixerVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SfxKey, value);
    }
}
