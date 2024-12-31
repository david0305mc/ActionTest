using Unity.VisualScripting;
using UnityEngine;

public class MelleWeeaponDetector : MonoBehaviour
{

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var unitObj = collision.gameObject.GetComponent<UnitObj>();
    //    if (unitObj != null)
    //    {
    //        unitObj.Damaged();
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var unitObj = collision.gameObject.GetComponent<UnitObj>();
    //    if (unitObj != null)
    //    {
    //        unitObj.Damaged();
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        var unitObj = other.gameObject.GetComponent<UnitObj>();
        if (unitObj != null)
        {
            unitObj.Damaged();
        }
    }
}
