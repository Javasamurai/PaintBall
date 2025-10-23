using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class UIVirtualButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class Event : UnityEvent { }

    [InputControl(layout = "Button")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        SendValueToControl(1.0f);
        Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SendValueToControl(0.0f);
        Debug.Log("OnPointerUp");
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SendValueToControl(1.0f);
        Debug.Log("OnPointerClick");
    }

}
