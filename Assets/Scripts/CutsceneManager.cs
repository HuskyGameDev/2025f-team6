using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)) {
            LoadScene("Main");
        }
    }

    // public void SkipCutscene() {
    //     director.time = director.duration; 
    //     director.Evaluate(); 
    // }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
