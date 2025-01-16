using System.Collections.Generic;
using UnityEngine;

public class MultiBezierCurve : MonoBehaviour
{
    [Header("Control Points")]
    public List<Transform> controlPoints; // ������ ����Ʈ
    [Header("Curve Resolution")]
    public int resolution = 50; // ��� ���ø� ����Ʈ ����
    public LineRenderer lineRenderer;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        DrawBezierCurve();
    }

    /// <summary>
    /// ������ � ���
    /// </summary>
    /// <param name="t">t �Ķ���� (0~1)</param>
    /// <returns>������ ����� ��</returns>
    private Vector3 CalculateBezierPoint(float t)
    {
        int n = controlPoints.Count - 1; // �������� ���� - 1 (����)
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
    /// ���� ��� ���
    /// </summary>
    /// <param name="n">����</param>
    /// <param name="k">�׸� ��ȣ</param>
    /// <returns>���� ���</returns>
    private float BinomialCoefficient(int n, int k)
    {
        return Factorial(n) / (Factorial(k) * Factorial(n - k));
    }

    /// <summary>
    /// ���丮�� ���
    /// </summary>
    /// <param name="n">�Է� ��</param>
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
    /// ������ � �׸���
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
