using UnityEngine;
using UnityEngine.EventSystems;

public class HoverShowObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject target;

    void Start()
    {
        if (target) target.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (target) target.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (target) target.SetActive(false);
    }
}
