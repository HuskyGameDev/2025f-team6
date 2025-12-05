using UnityEngine;
using UnityEngine.UI;

public class DifficultyButtonManager : MonoBehaviour
{
    public static int difficultyValue = 1;

    private Button[] difficultyButtons;
    private CanvasGroup[] buttonGroups;

    [Range(0f, 1f)]
    public float fadedAlpha = 0.35f;     // Transparency for NON-selected buttons
    public float selectedAlpha = 1f;     // Full opacity for selected option

    private void Start()
    {
        // Load all difficulty-tagged buttons
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Difficulty");

        difficultyButtons = new Button[taggedObjects.Length];
        buttonGroups = new CanvasGroup[taggedObjects.Length];

        for (int i = 0; i < taggedObjects.Length; i++)
        {
            difficultyButtons[i] = taggedObjects[i].GetComponent<Button>();

            // Ensure each button has a CanvasGroup for transparency control
            CanvasGroup cg = taggedObjects[i].GetComponent<CanvasGroup>();
            if (cg == null)
                cg = taggedObjects[i].AddComponent<CanvasGroup>();

            buttonGroups[i] = cg;
        }

        UpdateVisuals();
    }

    public void SetEasy()
    {
        difficultyValue = 1;
        UpdateVisuals();
    }

    public void SetMedium()
    {
        difficultyValue = 2;
        UpdateVisuals();
    }

    public void SetHard()
    {
        difficultyValue = 3;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            string name = difficultyButtons[i].gameObject.name.ToLower();

            bool selected =
                (difficultyValue == 1 && name.Contains("easy")) ||
                (difficultyValue == 2 && name.Contains("medium")) ||
                (difficultyValue == 3 && name.Contains("hard"));

            buttonGroups[i].alpha = selected ? selectedAlpha : fadedAlpha;
        }
    }
}
