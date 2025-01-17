using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BezierArrow2 : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    private List<Vector3> controlPoints;
    public float speed = 30f; // �̻��� �ӵ�
    
    public async UniTask Fire(Vector3 _target0, Vector3 _target1)
    {
        controlPoints = new List<Vector3>();
        controlPoints.Add(transform.position);
        controlPoints.Add(_target0);
        controlPoints.Add(_target1);
        
        float curveLength = CalculateBezierCurveLength(5);

        float timeElapse = 0f;

        while (timeElapse < 1f)
        {
            float nextElpase = timeElapse + Time.deltaTime / curveLength * speed;
            if (nextElpase > 1f)
                nextElpase = 1f;

            Vector3 pos = CalculateBezierPoint(timeElapse);
            Vector3 nextPos = CalculateBezierPoint(nextElpase);

            rigidBody.Move(pos, Quaternion.LookRotation(nextPos - pos));
            //transform.rotation = Quaternion.LookRotation(nextPos - pos);
            timeElapse = nextElpase;
            await UniTask.Yield();
        }

        // After Dst
        timeElapse = 0f;
        while (timeElapse < 100f)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            timeElapse += Time.deltaTime;
            await UniTask.Yield();
        }
    }

    private float CalculateBezierCurveLength(int _resolution)
    {
        float length = 0f;
        Vector3 previousPoint = controlPoints[0];
        for (int i = 1; i <= _resolution; i++)
        {
            float t = i / (float)_resolution;
            Vector3 currPos = CalculateBezierPoint(t);
            length += Vector3.Distance(previousPoint, currPos);
            previousPoint = currPos;
        }
        return length;
    }
    private Vector3 CalculateBezierPoint(float t)
    {
        int n = controlPoints.Count - 1; // �������� ���� - 1 (����)
        Vector3 point = Vector3.zero;

        for (int i = 0; i <= n; i++)
        {
            float binomialCoefficient = BinomialCoefficient(n, i);
            float term = binomialCoefficient * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            point += term * controlPoints[i];
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

}
