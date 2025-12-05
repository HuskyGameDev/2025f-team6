using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; }

    private static KeyCode moveLeft1 = KeyCode.A;
    private static KeyCode moveLeft2 = KeyCode.LeftArrow;
    private static KeyCode moveRight1 = KeyCode.D;
    private static KeyCode moveRight2 = KeyCode.RightArrow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public static void SetMoveLeft1(KeyCode newKey)
    {
        moveLeft1 = newKey;
    }

    public static void SetMoveLeft2(KeyCode newKey)
    {
        moveLeft2 = newKey;
    }

    public static void SetMoveRight1(KeyCode newKey)
    {
        moveRight1 = newKey;
    }

    public static void SetMoveRight2(KeyCode newKey)
    {
        moveRight2 = newKey;
    }

    public static KeyCode GetMoveLeft1()
    {
        return moveLeft1;
    }

    public static KeyCode GetMoveLeft2()
    {
        return moveLeft2;
    }

    public static KeyCode GetMoveRight1()
    {
        return moveRight1;
    }

    public static KeyCode GetMoveRight2()
    {
        return moveRight2;
    }
}