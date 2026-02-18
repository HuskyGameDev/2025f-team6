using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RebindMenuController : MonoBehaviour
{
    public TMP_Text moveLeftText;
    public TMP_Text moveRightText;
    public TMP_Text usePowerupText;
    public TMP_Text honkHornText;

    public Button rebindMoveLeftButton;
    public Button rebindMoveRightButton;
    public Button rebindUsePowerupButton;
    public Button rebindHonkHornButton;

    public Button returnButton;
    public Button resetDefaultsButton;

    private SystemSystem target = SystemSystem.None;

    private enum SystemSystem
    {
        None,
        MoveLeft,
        MoveRight,
        UsePowerup,
        HonkHorn
    }

    void Start()
    {
        if (rebindMoveLeftButton) rebindMoveLeftButton.onClick.AddListener(() => BeginRebind(SystemSystem.MoveLeft));
        if (rebindMoveRightButton) rebindMoveRightButton.onClick.AddListener(() => BeginRebind(SystemSystem.MoveRight));
        if (rebindUsePowerupButton) rebindUsePowerupButton.onClick.AddListener(() => BeginRebind(SystemSystem.UsePowerup));
        if (rebindHonkHornButton) rebindHonkHornButton.onClick.AddListener(() => BeginRebind(SystemSystem.HonkHorn));

        if (returnButton) returnButton.onClick.AddListener(ReturnToSettings);
        if (resetDefaultsButton) resetDefaultsButton.onClick.AddListener(ResetDefaults);

        RefreshUI();
        UpdateReturnInteractable();
    }

    void Update()
    {
        if (target == SystemSystem.None) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            target = SystemSystem.None;
            RefreshUI();
            UpdateReturnInteractable();
            return;
        }

        KeyCode pressed = GetPressedKey();
        if (pressed == KeyCode.None) return;

        ApplyWithConflict(pressed);
        target = SystemSystem.None;

        RefreshUI();
        UpdateReturnInteractable();
    }

    private void BeginRebind(SystemSystem which)
    {
        target = which;
        UpdateReturnInteractable();
    }

    private void ApplyWithConflict(KeyCode key)
    {
        System.Func<KeyCode>[] getters =
        {
            KeybindManager.GetMoveLeft1,
            KeybindManager.GetMoveRight1,
            KeybindManager.GetUsePowerup,
            KeybindManager.GetHonkHorn
        };

        System.Action<KeyCode>[] setters =
        {
            KeybindManager.SetMoveLeft1,
            KeybindManager.SetMoveRight1,
            KeybindManager.SetUsePowerup,
            KeybindManager.SetHonkHorn
        };

        int idx = -1;
        System.Action<KeyCode> setTarget = null;

        switch (target)
        {
            case SystemSystem.MoveLeft: idx = 0; setTarget = KeybindManager.SetMoveLeft1; break;
            case SystemSystem.MoveRight: idx = 1; setTarget = KeybindManager.SetMoveRight1; break;
            case SystemSystem.UsePowerup: idx = 2; setTarget = KeybindManager.SetUsePowerup; break;
            case SystemSystem.HonkHorn: idx = 3; setTarget = KeybindManager.SetHonkHorn; break;
        }

        if (setTarget == null) return;

        KeybindManager.UnbindFirst(key, getters, setters, idx);
        setTarget(key);
    }

    private void ResetDefaults()
    {
        target = SystemSystem.None;
        KeybindManager.ResetDefaults();
        RefreshUI();
        UpdateReturnInteractable();
    }

    private void ReturnToSettings()
    {
        if (target != SystemSystem.None) return;
        if (!KeybindManager.AllBound()) return;
        SceneManager.LoadScene("OptionsMenu");
    }

    private void UpdateReturnInteractable()
    {
        if (!returnButton) returnButton = null;
        if (returnButton) returnButton.interactable = (target == SystemSystem.None && KeybindManager.AllBound());
    }

    private void RefreshUI()
    {
        if (moveLeftText)
            moveLeftText.text = KeybindManager.GetMoveLeft1().ToString();

        if (moveRightText)
            moveRightText.text = KeybindManager.GetMoveRight1().ToString();

        if (usePowerupText)
            usePowerupText.text = KeybindManager.GetUsePowerup().ToString();

        if (honkHornText)
            honkHornText.text = KeybindManager.GetHonkHorn().ToString();
    }


    private KeyCode GetPressedKey()
    {
        foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (k == KeyCode.None) continue;
            if (Input.GetKeyDown(k)) return k;
        }
        return KeyCode.None;
    }
}

//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using TMPro;

//public class RebindMenuController : MonoBehaviour
//{
//    public TMP_Text moveLeftText;
//    public TMP_Text moveRightText;
//    public TMP_Text usePowerupText;
//    public TMP_Text honkHornText;

//    public Button rebindMoveLeftButton;
//    public Button rebindMoveRightButton;
//    public Button rebindUsePowerupButton;
//    public Button rebindHonkHornButton;

//    public Button returnButton;
//    public Button resetDefaultsButton;

//    public TMP_Text warningText;

//    private Target target = Target.None;

//    private string moveLeftBase;
//    private string moveRightBase;
//    private string usePowerupBase;
//    private string honkHornBase;

//    private enum Target
//    {
//        None,
//        MoveLeft,
//        MoveRight,
//        UsePowerup,
//        HonkHorn
//    }

//    void Start()
//    {
//        if (moveLeftText) moveLeftBase = moveLeftText.text;
//        if (moveRightText) moveRightBase = moveRightText.text;
//        if (usePowerupText) usePowerupBase = usePowerupText.text;
//        if (honkHornText) honkHornBase = honkHornText.text;

//        if (rebindMoveLeftButton) rebindMoveLeftButton.onClick.AddListener(() => BeginRebind(Target.MoveLeft));
//        if (rebindMoveRightButton) rebindMoveRightButton.onClick.AddListener(() => BeginRebind(Target.MoveRight));
//        if (rebindUsePowerupButton) rebindUsePowerupButton.onClick.AddListener(() => BeginRebind(Target.UsePowerup));
//        if (rebindHonkHornButton) rebindHonkHornButton.onClick.AddListener(() => BeginRebind(Target.HonkHorn));

//        if (returnButton) returnButton.onClick.AddListener(ReturnToSettings);
//        if (resetDefaultsButton) resetDefaultsButton.onClick.AddListener(ResetDefaults);

//        HideWarning();
//        RefreshUI();
//    }

//    void Update()
//    {
//        if (target == Target.None) return;

//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            target = Target.None;
//            HideWarning();
//            RefreshUI();
//            return;
//        }

//        KeyCode pressed = GetPressedKey();
//        if (pressed == KeyCode.None) return;

//        ApplyWithConflict(pressed);
//        target = Target.None;

//        HideWarning();
//        RefreshUI();
//    }

//    private void BeginRebind(Target which)
//    {
//        target = which;
//        HideWarning();
//        RefreshUI();
//    }

//    private void ApplyWithConflict(KeyCode key)
//    {
//        System.Func<KeyCode>[] getters =
//        {
//            KeybindManager.GetMoveLeft1,
//            KeybindManager.GetMoveRight1,
//            KeybindManager.GetUsePowerup,
//            KeybindManager.GetHonkHorn
//        };

//        System.Action<KeyCode>[] setters =
//        {
//            KeybindManager.SetMoveLeft1,
//            KeybindManager.SetMoveRight1,
//            KeybindManager.SetUsePowerup,
//            KeybindManager.SetHonkHorn
//        };

//        int idx = -1;
//        System.Action<KeyCode> setTarget = null;

//        switch (target)
//        {
//            case Target.MoveLeft: idx = 0; setTarget = KeybindManager.SetMoveLeft1; break;
//            case Target.MoveRight: idx = 1; setTarget = KeybindManager.SetMoveRight1; break;
//            case Target.UsePowerup: idx = 2; setTarget = KeybindManager.SetUsePowerup; break;
//            case Target.HonkHorn: idx = 3; setTarget = KeybindManager.SetHonkHorn; break;
//        }

//        if (setTarget == null) return;

//        KeybindManager.UnbindFirst(key, getters, setters, idx);
//        setTarget(key);
//    }

//    private void ResetDefaults()
//    {
//        target = Target.None;
//        KeybindManager.ResetDefaults();
//        HideWarning();
//        RefreshUI();
//    }

//    private void ReturnToSettings()
//    {
//        if (target != Target.None) return;

//        if (!KeybindManager.AllBound())
//        {
//            ShowWarning("All controls must be binded to continue!");
//            return;
//        }

//        HideWarning();

//        if (!KeybindManager.AllBound()) return; else SceneManager.LoadScene("OptionsMenu");
//    }

//    private void RefreshUI()
//    {
//        if (moveLeftText) moveLeftText.text = moveLeftBase + " " + (target == Target.MoveLeft ? "..." : Display(KeybindManager.GetMoveLeft1()));
//        if (moveRightText) moveRightText.text = moveRightBase + " " + (target == Target.MoveRight ? "..." : Display(KeybindManager.GetMoveRight1()));
//        if (usePowerupText) usePowerupText.text = usePowerupBase + " " + (target == Target.UsePowerup ? "..." : Display(KeybindManager.GetUsePowerup()));
//        if (honkHornText) honkHornText.text = honkHornBase + " " + (target == Target.HonkHorn ? "..." : Display(KeybindManager.GetHonkHorn()));
//    }

//    private string Display(KeyCode k)
//    {
//        return k == KeyCode.None ? "None" : k.ToString();
//    }

//    private void ShowWarning(string msg)
//    {
//        if (!warningText) return;
//        warningText.text = msg;
//        warningText.gameObject.SetActive(true);
//    }

//    private void HideWarning()
//    {
//        if (!warningText) return;
//        warningText.gameObject.SetActive(false);
//    }

//    private KeyCode GetPressedKey()
//    {
//        foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
//        {
//            if (k == KeyCode.None) continue;
//            if (k == KeyCode.Escape) continue;
//            if (Input.GetKeyDown(k)) return k;
//        }
//        return KeyCode.None;
//    }
//}
