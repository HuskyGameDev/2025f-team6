using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindManager : MonoBehaviour
{
    public static KeybindManager Instance { get; private set; }

    private static KeyCode moveLeft1 = KeyCode.A;
    private static KeyCode moveLeft2 = KeyCode.LeftArrow;
    private static KeyCode moveRight1 = KeyCode.D;
    private static KeyCode moveRight2 = KeyCode.RightArrow;
    private static KeyCode honkHorn = KeyCode.Space;
    private static KeyCode usePowerup = KeyCode.E;

    private static readonly KeyCode defaultMoveLeft1 = KeyCode.A;
    private static readonly KeyCode defaultMoveLeft2 = KeyCode.LeftArrow;
    private static readonly KeyCode defaultMoveRight1 = KeyCode.D;
    private static readonly KeyCode defaultMoveRight2 = KeyCode.RightArrow;
    private static readonly KeyCode defaultHonkHorn = KeyCode.Space;
    private static readonly KeyCode defaultUsePowerup = KeyCode.E;

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

    public static void SetMoveLeft1(KeyCode newKey) { moveLeft1 = newKey; }
    public static void SetMoveLeft2(KeyCode newKey) { moveLeft2 = newKey; }
    public static void SetMoveRight1(KeyCode newKey) { moveRight1 = newKey; }
    public static void SetMoveRight2(KeyCode newKey) { moveRight2 = newKey; }
    public static void SetHonkHorn(KeyCode newKey) { honkHorn = newKey; }
    public static void SetUsePowerup(KeyCode newKey) { usePowerup = newKey; }

    public static KeyCode GetMoveLeft1() { return moveLeft1; }
    public static KeyCode GetMoveLeft2() { return moveLeft2; }
    public static KeyCode GetMoveRight1() { return moveRight1; }
    public static KeyCode GetMoveRight2() { return moveRight2; }
    public static KeyCode GetHonkHorn() { return honkHorn; }
    public static KeyCode GetUsePowerup() { return usePowerup; }

    public static void ResetDefaults()
    {
        moveLeft1 = defaultMoveLeft1;
        moveLeft2 = defaultMoveLeft2;
        moveRight1 = defaultMoveRight1;
        moveRight2 = defaultMoveRight2;
        honkHorn = defaultHonkHorn;
        usePowerup = defaultUsePowerup;
    }

    public static bool AllBound()
    {
        return moveLeft1 != KeyCode.None &&
               moveLeft2 != KeyCode.None &&
               moveRight1 != KeyCode.None &&
               moveRight2 != KeyCode.None &&
               honkHorn != KeyCode.None &&
               usePowerup != KeyCode.None;
    }

    public static void UnbindFirst(KeyCode key, System.Func<KeyCode>[] getters, System.Action<KeyCode>[] setters, int skipIndex)
    {
        for (int i = 0; i < getters.Length; i++)
        {
            if (i == skipIndex) continue;
            if (getters[i]() == key)
            {
                setters[i](KeyCode.None);
                return;
            }
        }
    }
}
