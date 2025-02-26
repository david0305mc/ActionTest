using UnityEngine;


public class CurvedMissile : MonoBehaviour
{
    public Transform startPoint; // 미사일 출발 위치
    public Transform controlPoint; // 곡선을 형성할 중간 지점
    public Transform targetPoint; // 목표 위치
    public float speed = 2f; // 미사일 속도

    private float t = 0f; // 0~1 사이 값을 증가시켜 곡선을 따라 이동

    void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime * speed; // 시간에 따라 t 값을 증가시켜 이동
            transform.position = CalculateBezierPoint(t, startPoint.position, controlPoint.position, targetPoint.position);
            transform.LookAt(CalculateBezierPoint(t + 0.05f, startPoint.position, controlPoint.position, targetPoint.position)); // 미사일 회전
        }
        else
        {
            Destroy(gameObject); // 목표에 도달하면 제거
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 2차 베지어 곡선 공식: (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}
