using UnityEngine;

public interface IAnimationEventFowarder
{
    public void OnAnimationEvent(AnimationEventType _eventType);
}

public class AnimationEventForwarder : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;

    private IAnimationEventFowarder animationForwarder;
    private void Awake()
    {
        if (parentObject == null)
        {
            Debug.LogError("parentObject == null");
            animationForwarder = null;
            return;
        }
        animationForwarder = parentObject.GetComponent<IAnimationEventFowarder>();
    }

    public void OnAnimationEvent(AnimationEventType _eventType)
    {
        animationForwarder?.OnAnimationEvent(_eventType);
    }

}
