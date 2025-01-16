using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPointBezierProjectile : MonoBehaviour
{
    public List<Transform> controlPoints; // 베지어 곡선을 따라 이동할 포지션 리스트
    public float speed = 10f; // 미사일의 이동 속도

    private float progress = 0f; // 곡선 진행도
    private bool isMoving = false; // 미사일 이동 상태

    void Start()
    {
        if (controlPoints.Count >= 2)
        {
            isMoving = true;
        }
        else
        {
            Debug.LogError("At least 2 control points are required.");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // 곡선 진행도 업데이트
            progress += speed * Time.deltaTime / CalculateCurveLength();

            // 베지어 곡선 경로 계산 및 이동
            Vector3 currentPosition = CalculateBezierPoint(progress, controlPoints);
            transform.position = currentPosition;

            // 이동이 끝났는지 확인
            if (progress >= 1f)
            {
                isMoving = false;
                OnReachTarget();
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, List<Transform> points)
    {
        if (points.Count == 1)
        {
            return points[0].position;
        }

        List<Transform> nextPoints = new List<Transform>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 interpolated = Vector3.Lerp(points[i].position, points[i + 1].position, t);
            GameObject temp = new GameObject();
            temp.transform.position = interpolated;
            nextPoints.Add(temp.transform);
        }

        Vector3 result = CalculateBezierPoint(t, nextPoints);

        // Temporary game objects cleanup
        foreach (Transform temp in nextPoints)
        {
            Destroy(temp.gameObject);
        }

        return result;
    }

    float CalculateCurveLength()
    {
        float length = 0f;
        Vector3 previousPoint = controlPoints[0].position;

        for (int i = 1; i <= 50; i++)
        {
            float t = i / 50f;
            Vector3 currentPoint = CalculateBezierPoint(t, controlPoints);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }

    void OnReachTarget()
    {
        // 목표 위치 도달 시 실행할 동작
        Debug.Log("Missile reached the final position.");
        Destroy(gameObject); // 미사일 제거
    }
}
