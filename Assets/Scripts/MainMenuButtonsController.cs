using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuButtonsController : MonoBehaviour
{
    public Button[] buttons;

    public Color defaultColor = Color.white;
    public Color hoverColor = new Color32(239, 195, 9, 255);
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (buttons != null)
        {
            foreach (Button b in buttons)
                SetButtonTextColor(b, defaultColor);

            // optional; highlight the first button by default
            if (buttons.Length > 0)
                SetButtonTextColor(buttons[0], hoverColor);
        }
    }


    public void OnButtonHover(Button hoveredButton)
    {
        if (buttons == null) return;

        foreach (Button b in buttons)
            SetButtonTextColor(b, defaultColor);

        SetButtonTextColor(hoveredButton, hoverColor);

        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnButtonExit(Button button)
    {
        SetButtonTextColor(button, defaultColor);
    }

    void SetButtonTextColor(Button button, Color color)
    {
        if (button == null) return;

        TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null)
        {
            tmp.color = color;
            return;
        }

        Debug.LogWarning("no text for MainMenuButtonController to change color of in button " + button.name);
    }


    public void PlayGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("Main");

    }

    public void OpenOptions()
    {
        PlayClickSound();
        SceneManager.LoadScene("OptionsMenu");
    }

    public void OpenAbout()
    {
        PlayClickSound();
        SceneManager.LoadScene("AboutMenu");
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("game will be able to quit here once built.");
        Application.Quit();
    }

    public void PlayClickSound()
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}
