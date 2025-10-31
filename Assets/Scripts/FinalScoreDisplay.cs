using TMPro;
using UnityEngine;

public class FinalScoreDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextMeshProUGUI tmp = GetComponentInChildren<TextMeshProUGUI>();
        tmp.SetText(UIController.getFinalScore().ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
