using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
    }
}
