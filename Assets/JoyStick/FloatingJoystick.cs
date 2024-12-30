using UnityEngine;

public class FloatingJoystick : MonoBehaviour
{
    public RectTransform joyStickObj;
    public RectTransform Knob;

    private void Awake()
    {
        joyStickObj = GetComponent<RectTransform>();
    }
}
