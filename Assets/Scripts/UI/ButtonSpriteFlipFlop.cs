using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class ButtonSpriteFlipFlop
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
{
    public enum Mode
    {
        NormalHoverPressed, // Normal -> Hover on hover, Pressed while held down
        NormalPressedOnly, // Normal -> Pressed while held down
        ToggleOnClick, // Click toggles Normal <-> Pressed
        ToggleWithObject, // Sprite follows toggleObject.activeSelf (On=Pressed, Off=Normal)
    }

    [Header("Target")]
    [SerializeField]
    private Image targetImage;

    [SerializeField, Tooltip("Optional. If assigned, pointer actions respect interactable state.")]
    private Button button;

    [Header("Sprites")]
    [SerializeField]
    private Sprite normalSprite;

    [SerializeField]
    private Sprite hoverSprite;

    [SerializeField]
    private Sprite pressedSprite;

    [Header("Behavior")]
    [SerializeField]
    private Mode mode = Mode.NormalHoverPressed;

    [SerializeField]
    private bool applyHover = true;

    [SerializeField]
    private bool applyPressed = true;

    [Header("ToggleWithObject")]
    [SerializeField]
    private GameObject toggleObject;

    [SerializeField, Tooltip("Invert the meaning of 'active'.")]
    private bool invertToggleObjectState;

    [SerializeField, Tooltip("If true, clicking will toggle the GameObject active state.")]
    private bool driveToggleObjectOnClick;

    [Header("ToggleOnClick")]
    [SerializeField]
    private bool startToggled;

    private bool isHovered;
    private bool isPressed;
    private bool isToggled;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        // If you don't set a normal sprite, capture whatever is currently on the Image.
        if (targetImage != null && normalSprite == null)
            normalSprite = targetImage.sprite;

        isToggled = startToggled;
        RefreshSprite();
    }

    private void OnEnable()
    {
        RefreshSprite();
    }

    private void Update()
    {
        // Keep synced if another script toggles the object.
        if (mode == Mode.ToggleWithObject)
            RefreshSprite();
    }

    private bool CanInteract()
    {
        if (button == null)
            return true;
        return button.interactable && button.gameObject.activeInHierarchy;
    }

    public void OnPointerEnter(PointerEventData _)
    {
        isHovered = true;
        RefreshSprite();
    }

    public void OnPointerExit(PointerEventData _)
    {
        isHovered = false;
        isPressed = false;
        RefreshSprite();
    }

    public void OnPointerDown(PointerEventData _)
    {
        if (!CanInteract())
            return;

        isPressed = true;
        RefreshSprite();
    }

    public void OnPointerUp(PointerEventData _)
    {
        if (!CanInteract())
            return;

        bool click = isPressed && isHovered;
        isPressed = false;

        if (click)
        {
            if (mode == Mode.ToggleOnClick)
            {
                isToggled = !isToggled;
            }
            else if (mode == Mode.ToggleWithObject)
            {
                if (driveToggleObjectOnClick && toggleObject != null)
                    toggleObject.SetActive(!toggleObject.activeSelf);
            }
        }

        RefreshSprite();
    }

    private void RefreshSprite()
    {
        if (targetImage == null)
            return;

        // 1) Press feedback
        if (isPressed && applyPressed && pressedSprite != null)
        {
            targetImage.sprite = pressedSprite;
            return;
        }

        // 2) ToggleWithObject
        if (mode == Mode.ToggleWithObject)
        {
            bool on = false;

            if (toggleObject != null)
                on = toggleObject.activeSelf;

            if (invertToggleObjectState)
                on = !on;

            if (!on && isHovered && applyHover && hoverSprite != null)
            {
                targetImage.sprite = hoverSprite;
                return;
            }

            if (on && pressedSprite != null)
            {
                targetImage.sprite = pressedSprite;
                return;
            }

            if (normalSprite != null)
            {
                targetImage.sprite = normalSprite;
                return;
            }

            return;
        }

        // 3) ToggleOnClick
        if (mode == Mode.ToggleOnClick)
        {
            if (!isToggled && isHovered && applyHover && hoverSprite != null)
            {
                targetImage.sprite = hoverSprite;
                return;
            }

            if (isToggled && pressedSprite != null)
            {
                targetImage.sprite = pressedSprite;
                return;
            }

            if (normalSprite != null)
            {
                targetImage.sprite = normalSprite;
                return;
            }

            return;
        }

        if (mode == Mode.NormalHoverPressed)
        {
            if (isHovered && applyHover && hoverSprite != null)
            {
                targetImage.sprite = hoverSprite;
                return;
            }
        }

        if (normalSprite != null)
            targetImage.sprite = normalSprite;
    }

    public void SetToggled(bool toggled)
    {
        isToggled = toggled;
        RefreshSprite();
    }

    public bool GetToggled()
    {
        return isToggled;
    }
}
