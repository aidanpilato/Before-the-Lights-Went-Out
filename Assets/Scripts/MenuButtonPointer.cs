using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.SetHover(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MenuManager.Instance.ClearHover(transform);
    }
}