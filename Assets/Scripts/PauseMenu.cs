using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite frame0;
    [SerializeField] Sprite frame1;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void Pause()
    {   
        if (frame1 != null) buttonImage.sprite = frame1;

        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        if (frame0 != null) buttonImage.sprite = frame0;

        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void TogglePause()
    {
        if (pauseMenu.activeSelf)
            {
                Resume();
            }
            else
            {
                Pause();
            }
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

}
