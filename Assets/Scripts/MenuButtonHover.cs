using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Drawing;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainMenuButtonsController menuController;
    public RawImage icon;

    public Texture2D BaseSprite;
    public Texture2D HoverSprite;


    Button myButton;

    void Awake()
    {
        WarnNull();
        myButton = GetComponent<Button>();


        //if (icon.name != "RawImageA")
        //{
        //    icon.enabled = false;
        //}
        //if (icon) icon.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (menuController != null && myButton != null)
        {
            if (icon != null)
            {
                icon.texture = HoverSprite;
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
                icon.texture = BaseSprite;
            }
            
            menuController.OnButtonExit(myButton);
        }
    }

    public void WarnNull()
    {
        if (icon == null) Debug.LogWarning("Icon RawImage not assigned in MenuButtonHover on " + gameObject.name);
        if (BaseSprite == null) Debug.LogWarning("BaseSprite not assigned in MenuButtonHover on " + gameObject.name);
        if (HoverSprite == null) Debug.LogWarning("HoverSprite not assigned in MenuButtonHover on " + gameObject.name);
    }
}
