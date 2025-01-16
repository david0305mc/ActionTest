using System.Collections.Generic;
using UnityEngine;

public class MissileBezierPrediction : MonoBehaviour
{
    public float speed = 10f; // 미사일 속도
    public float predictionDistance = 20f; // 예측 거리
    public float historyDuration = 1f; // 기록할 시간 (1초)

    private Queue<Vector3> positionHistory = new Queue<Vector3>(); // 위치 기록
    private Queue<Vector3> directionHistory = new Queue<Vector3>(); // 방향 기록

    private float timer = 0f;

    void Update()
    {
        // 미사일 이동
        MoveMissile();

        // 현재 위치 및 방향 기록
        RecordMissileMovement();

        // Bezier 경로 예측 (P 키로 실행)
        if (Input.GetKeyDown(KeyCode.P))
        {
            PredictBezierPath();
        }
    }

    private void RecordMissileMovement()
    {
        timer += Time.deltaTime;

        // 현재 위치와 방향 기록
        positionHistory.Enqueue(transform.position);
        directionHistory.Enqueue(transform.forward);

        // 오래된 데이터 삭제
        if (timer > historyDuration)
        {
            positionHistory.Dequeue();
            directionHistory.Dequeue();
        }
    }

    private void MoveMissile()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void PredictBezierPath()
    {
        if (positionHistory.Count < 2 || directionHistory.Count < 2)
        {
            Debug.LogWarning("충분한 데이터를 기록 중입니다. 잠시 기다려 주세요.");
            return;
        }

        // 시작점 (P0)
        Vector3 startPoint = transform.position;

        // 제어점 (P1) 계산 - 평균 방향으로 예측
        Vector3 averageDirection = Vector3.zero;
        foreach (var dir in directionHistory)
        {
            averageDirection += dir;
        }
        averageDirection.Normalize();
        Vector3 controlPoint = startPoint + averageDirection * predictionDistance * 0.5f;

        // 끝점 (P2) 계산 - 평균 방향으로 일정 거리 떨어진 지점
        Vector3 endPoint = startPoint + averageDirection * predictionDistance;

        // Bezier 곡선 계산 및 시각화
        Debug.Log("Predicting Bezier Path...");
        int segments = 50; // 곡선의 세분화 정도
        Vector3 previousPoint = startPoint;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 currentPoint = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);

            // 곡선을 따라 선을 그립니다.
            Debug.DrawLine(previousPoint, currentPoint, Color.red, 2f);
            previousPoint = currentPoint;
        }
    }

    // Bezier 곡선의 점 계산
    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}
