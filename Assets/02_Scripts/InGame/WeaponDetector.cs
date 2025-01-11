using UnityEngine;

public class WeaponDetector : MonoBehaviour
{

    private System.Action<Collider> triggerEnterAction;

    public void SetOnTriggerEnter(System.Action<Collider> _action)
    {
        triggerEnterAction = _action;
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnterAction?.Invoke(other);
    }
}
