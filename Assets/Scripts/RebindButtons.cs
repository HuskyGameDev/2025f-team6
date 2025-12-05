//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections;
//using System;

//public class RebindButtons : MonoBehaviour
//{
//    [SerializeField] PlayerControl controlScript;
//    private bool listening = false;
//    private KeyCode[] values;
//    private bool[] keys;
//    private Button button;
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        values = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
//        keys = new bool[values.Length];
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (listening & Input.anyKeyDown)
//        {
//            for (int i = 0; i < values.Length; i++)
//            {
//                keys[i] = Input.GetKey((KeyCode)values[i]);
//                if (keys[i])
//                {
//                    Debug.Log(values[i].ToString());
//                    listening = false;
//                    button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
//                    Debug.Log(button.name);
//                    switch (button.name)
//                    {
//                        case "MoveLeft1Button":
//                            KeybindManager.SetMoveLeft1(values[i]);
//                            button.GetComponentInChildren<TextMeshProUGUI>().text = "Move Left: " + values[i].ToString();
//                            break;
//                        case "MoveRight1Button":
//                            KeybindManager.SetMoveRight1(values[i]);
//                            button.GetComponentInChildren<TextMeshProUGUI>().text = "Move Right: " + values[i].ToString();
//                            break;
//                        default:
//                            break;
//                    }
//                }
//            }
//        }
//    }

//    public void ClickRebind(Button clicked)
//    {
//        listening = true;
//        button = clicked;
//        button.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(239, 195, 9, 255);
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebindButtons : MonoBehaviour
{
    [SerializeField] PlayerControl controlScript;

    [Header("Buttons")]
    [SerializeField] private Button moveLeft1Button;
    [SerializeField] private Button moveRight1Button;

    private bool listening = false;
    private KeyCode[] values;
    private bool[] keys;
    private Button button;

    void Start()
    {
        values = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        keys = new bool[values.Length];

        UpdateAllButtonTexts();   // <-- IMPORTANT FIX
    }

    private void UpdateAllButtonTexts()
    {
        // Pull stored bindings and update button labels
        moveLeft1Button.GetComponentInChildren<TextMeshProUGUI>().text =
            "Move Left: " + KeybindManager.GetMoveLeft1().ToString();

        moveRight1Button.GetComponentInChildren<TextMeshProUGUI>().text =
            "Move Right: " + KeybindManager.GetMoveRight1().ToString();
    }

    void Update()
    {
        if (listening && Input.anyKeyDown)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (Input.GetKey(values[i]))
                {
                    listening = false;
                    button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                    switch (button.name)
                    {
                        case "MoveLeft1Button":
                            KeybindManager.SetMoveLeft1(values[i]);
                            button.GetComponentInChildren<TextMeshProUGUI>().text =
                                "Move Left: " + values[i].ToString();
                            break;

                        case "MoveRight1Button":
                            KeybindManager.SetMoveRight1(values[i]);
                            button.GetComponentInChildren<TextMeshProUGUI>().text =
                                "Move Right: " + values[i].ToString();
                            break;
                    }
                }
            }
        }
    }

    public void ClickRebind(Button clicked)
    {
        listening = true;
        button = clicked;

        button.GetComponentInChildren<TextMeshProUGUI>().color =
            new Color32(239, 195, 9, 255);
    }
}
