using TMPro;
using UnityEngine;

public class FinalScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI yourScoreText;
    public TextMeshProUGUI highScoreText;

    private const string HighScoreKey = "HighScore";

    void Start()
    {
        int finalScore = UIController.getFinalScore();

        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        bool isNewHighScore = finalScore > highScore;

        if (isNewHighScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        yourScoreText.text = finalScore.ToString();
        highScoreText.text = highScore.ToString();

        if (isNewHighScore)
        {
            HighlightText(yourScoreText);
            HighlightText(highScoreText);
        }
    }

    void HighlightText(TextMeshProUGUI text)
    {
        text.color = Color.yellow;

        // bold
        text.fontStyle = FontStyles.Bold;
    }
}
