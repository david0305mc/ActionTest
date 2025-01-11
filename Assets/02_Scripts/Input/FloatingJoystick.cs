using UnityEngine;

public class FloatingJoystick : MonoBehaviour
{
    public RectTransform JoyStickObj { get; private set; }
    public RectTransform Knob;

    private void Awake()
    {
        JoyStickObj = GetComponent<RectTransform>();
    }
}
