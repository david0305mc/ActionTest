using UnityEngine;

public class BezierArrow : MonoBehaviour
{
    public Vector3 pointA; // 시작점 (A)
    public Vector3 pointB; // 통과점 (B)
    public Vector3 pointC; // 진행 방향의 끝점 (C)
    public float speed = 1f; // 화살의 이동 속도

    private float t = 0f; // Bezier 곡선의 진행 변수 (0~1)

    void Update()
    {
        if (t <= 1f)
        {
            // Bezier 곡선을 따라 화살 위치 업데이트
            Vector3 position = CalculateBezierPoint(t, pointA, pointB, pointC);
            transform.position = position;

            // Bezier 곡선을 따라 회전 (방향)
            Vector3 direction = CalculateBezierDirection(t, pointA, pointB, pointC);
            transform.rotation = Quaternion.LookRotation(direction);

            // t 증가 (이동)
            t += Time.deltaTime * speed;
        }
    }

    // Bezier 곡선의 위치 계산
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return point;
    }

    // Bezier 곡선의 방향 계산
    private Vector3 CalculateBezierDirection(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;

        Vector3 tangent = (2 * u * (p1 - p0)) + (2 * t * (p2 - p1));
        return tangent.normalized;
    }
}

