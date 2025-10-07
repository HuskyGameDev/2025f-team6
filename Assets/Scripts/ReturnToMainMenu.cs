using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
