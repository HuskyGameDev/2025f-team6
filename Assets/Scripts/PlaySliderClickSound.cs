//using UnityEngine;
//using UnityEngine.UI;
//public class SliderClickSound : MonoBehaviour
//{
//    public AudioSource uiAudio; // Assign your UIAudio AudioSource
//    public AudioClip clickSound; // Assign the click sound effect

//    private Slider slider;
//    void Awake()
//    {
//        slider = GetComponent<Slider>();
//        if (slider != null)
//            slider.onValueChanged.AddListener(PlayClick);
//    }

//    private void PlayClick(float value)
//    {
//        if (uiAudio != null && clickSound != null)
//            uiAudio.PlayOneShot(clickSound);
//    }
//}

using UnityEngine;
using UnityEngine.UI;
public class SliderClickSound : MonoBehaviour
{
    public AudioSource uiAudio;
    public AudioClip clickSound;
    public float debounceTime = 0.5f; // seconds between allowed plays

    private Slider slider;
    private float lastPlayTime = -999f;
    void Awake() {
        slider = GetComponent<Slider>();
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (Time.time - lastPlayTime >= debounceTime)
        {
            lastPlayTime = Time.time;
            if (uiAudio != null && clickSound != null) uiAudio.PlayOneShot(clickSound);
        }
    }
}