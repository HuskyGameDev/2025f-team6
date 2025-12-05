using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject runner;
    [SerializeField] private GameObject coiner;
    public Component[] temp;
    public TextMeshProUGUI scoreDisplay;
    private PointCounter scoreScript;
    private int score;
    public TextMeshProUGUI coinDisplay;
    private int coins = 0;
    private static int finalScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreScript = runner.GetComponent<PointCounter>();
        temp = GetComponentsInChildren<Component>();
        scoreDisplay = GetComponentInChildren<TextMeshProUGUI>();
        coinDisplay = coiner.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        score = scoreScript.GetPoints();
        finalScore = score;
        scoreDisplay.SetText("Score: " + score);

        coins = PlayerControl.coinsCollected;
        coinDisplay.SetText("" + coins);
    }

    public static void setFinalScore(int fscore)
    {
        finalScore = fscore;
    }

    public static int getFinalScore()
    {
        return finalScore;
    }
}
