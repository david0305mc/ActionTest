using System.Collections;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] RectTransform _targetUI = null;
    float speed = 0.2f;
    void Start()
    {
        MoveToUI(_targetUI);
    }
    public void MoveToUI(RectTransform targetUI)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(targetUI, targetUI.position, Camera.main, out Vector3 targetPos))
        {
            StartCoroutine(Move(targetPos));
        }
    }

    IEnumerator Move(Vector3 pos)
    {
        Vector3 dir = pos - transform.position; ;
        while (Vector3.Distance(transform.position, pos) > 0.1f)
        {
            transform.position += dir * speed * Time.deltaTime;
            yield return null;
        }

        Debug.Log("Arrive!");
    }
}