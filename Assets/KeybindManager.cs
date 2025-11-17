using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; }

    private KeyCode moveLeft1 = KeyCode.A;
    private KeyCode moveLeft2 = KeyCode.LeftArrow;
    private KeyCode moveRight1 = KeyCode.D;
    private KeyCode moveRight2 = KeyCode.RightArrow;

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

    public void SetMoveLeft1(KeyCode newKey)
    {
        moveLeft1 = newKey;
    }

    public void SetMoveLeft2(KeyCode newKey)
    {
        moveLeft2 = newKey;
    }

    public void SetMoveRight1(KeyCode newKey)
    {
        moveRight1 = newKey;
    }

    public void SetMoveRight2(KeyCode newKey)
    {
        moveRight2 = newKey;
    }

    public KeyCode GetMoveLeft1()
    {
        return moveLeft1;
    }

    public KeyCode GetMoveLeft2()
    {
        return moveLeft2;
    }

    public KeyCode GetMoveRight1()
    {
        return moveRight1;
    }

    public KeyCode GetMoveRight2()
    {
        return moveRight2;
    }
}