using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenuButtonsController menuController;
    public RawImage icon;

    Button myButton;

    void Awake()
    {
        myButton = GetComponent<Button>();
        //if (icon.name != "RawImageA")
        //{
        //    icon.enabled = false;
        //}
        icon.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuController != null && myButton != null)
        {
            if (icon != null)
            {
                icon.enabled = true;
            }

            menuController.OnButtonHover(myButton);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (menuController != null && myButton != null)
        {
            if (icon != null)
            {
                icon.enabled = false;
            }
            menuController.OnButtonExit(myButton);
        }
    }
}
