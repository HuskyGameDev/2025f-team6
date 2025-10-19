using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject runner;
    public Component[] temp;
    public TextMeshProUGUI scoreDisplay;
    private PointCounter scoreScript;
    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreScript = runner.GetComponent<PointCounter>();
        temp = GetComponentsInChildren<Component>();
        scoreDisplay = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        score = scoreScript.GetPoints();
        scoreDisplay.SetText("Score: " + score);
    }
}
