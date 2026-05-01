using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonPointer : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuManager.Instance.SetHover(transform);
    }
}