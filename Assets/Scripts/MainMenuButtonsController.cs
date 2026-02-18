using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuButtonsController : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] selectorIcons;

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
            for (int i = 0; i < buttons.Length; i++)
            {
                SetButtonTextColor(buttons[i], defaultColor);

                if (selectorIcons != null && i < selectorIcons.Length && selectorIcons[i] != null)
                    selectorIcons[i].SetActive(false);
            }
        }
    }

    public void OnButtonHover(Button hoveredButton)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            SetButtonTextColor(buttons[i], defaultColor);

            if (selectorIcons != null && i < selectorIcons.Length && selectorIcons[i] != null)
                selectorIcons[i].SetActive(false);
        }

        SetButtonTextColor(hoveredButton, hoverColor);

        int idx = System.Array.IndexOf(buttons, hoveredButton);
        if (selectorIcons != null && idx >= 0 && idx < selectorIcons.Length && selectorIcons[idx] != null)
            selectorIcons[idx].SetActive(true);

        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnButtonExit(Button button)
    {
        SetButtonTextColor(button, defaultColor);

        int idx = System.Array.IndexOf(buttons, button);
        if (selectorIcons != null && idx >= 0 && idx < selectorIcons.Length && selectorIcons[idx] != null)
            selectorIcons[idx].SetActive(false);
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
        SceneManager.LoadScene("VehicleMenu");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Intro Cutscene");
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

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("game will be able to quit here once built.");
        Application.Quit();
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}
