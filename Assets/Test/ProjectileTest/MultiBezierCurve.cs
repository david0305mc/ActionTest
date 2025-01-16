using System.Collections.Generic;
using UnityEngine;

public class MultiBezierCurve : MonoBehaviour
{
    [Header("Control Points")]
    public List<Transform> controlPoints; // 제어점 리스트
    [Header("Curve Resolution")]
    public int resolution = 50; // 곡선의 샘플링 포인트 개수
    public LineRenderer lineRenderer;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        DrawBezierCurve();
    }

    /// <summary>
    /// 비지어 곡선 계산
    /// </summary>
    /// <param name="t">t 파라미터 (0~1)</param>
    /// <returns>비지어 곡선상의 점</returns>
    private Vector3 CalculateBezierPoint(float t)
    {
        int n = controlPoints.Count - 1; // 제어점의 개수 - 1 (차수)
        Vector3 point = Vector3.zero;

        for (int i = 0; i <= n; i++)
        {
            float binomialCoefficient = BinomialCoefficient(n, i);
            float term = binomialCoefficient * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            point += term * controlPoints[i].position;
        }

        return point;
    }

    /// <summary>
    /// 이항 계수 계산
    /// </summary>
    /// <param name="n">차수</param>
    /// <param name="k">항목 번호</param>
    /// <returns>이항 계수</returns>
    private float BinomialCoefficient(int n, int k)
    {
        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }

    /// <summary>
    /// 팩토리얼 계산
    /// </summary>
    /// <param name="n">입력 값</param>
    /// <returns>n!</returns>
    private float Factorial(int n)
    {
        float result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    /// <summary>
    /// 비지어 곡선 그리기
    /// </summary>
    private void DrawBezierCurve()
    {
        if (controlPoints.Count < 2)
        {
            Debug.LogWarning("Control points must be at least 2!");
            return;
        }

        Vector3[] positions = new Vector3[resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            positions[i] = CalculateBezierPoint(t);
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
