using UnityEngine;

using Cysharp.Threading.Tasks;
using System.Threading;
using Cinemachine;
public class ProjectileLinearTest : MonoBehaviour
{
    public Transform target; // B���� (��ǥ ����)
    public float speed = 10f; // ȭ���� �ӵ�
    public float arcHeight = 5f; // ȭ���� ���ư��� ������ ������ ����

    private Vector3 startPoint; // A���� (��� ����)
    private Vector3 controlPoint; // �������� ����� ���� �߰� ����
    private float progress = 0f; // ���൵ (0 ~ 1)

    void Start()
    {
        startPoint = transform.position; // A���� ����
        if (target != null)
        {
            // �߰� �������� ��� (A������ B������ �߰� + ����)
            controlPoint = (startPoint + target.position) / 2;
            controlPoint += Vector3.up * arcHeight;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // ���൵ ������Ʈ
            progress += Time.deltaTime * speed / Vector3.Distance(startPoint, target.position);
            if (progress > 1f) progress = 1f;

            // ������ ��� ���� ȭ�� ��ġ ���
            Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, progress);
            Vector3 m2 = Vector3.Lerp(controlPoint, target.position, progress);
            transform.position = Vector3.Lerp(m1, m2, progress);

            // ȭ���� ��ǥ �������� ȸ��
            transform.LookAt(Vector3.Lerp(controlPoint, target.position, progress));

            // ��ǥ ������ �����ϸ� ȭ�� ����
            if (progress >= 1f)
            {
                OnHitTarget();
            }
        }
    }

    void OnHitTarget()
    {
        // ��ǥ�� ������ �� �ൿ ���� (ȿ��, �ı� ��)
        Debug.Log("Target Hit!");
        Destroy(gameObject);
    }

    //[SerializeField] private Rigidbody rigidBody;
    //private CancellationTokenSource cts;
    //private bool isDisposed;
    //public async UniTask Fire(Transform _target)
    //{
    //    isDisposed = false;
    //    cts = new CancellationTokenSource();
    //    Vector3 startPos = transform.position;
    //    while (true)
    //    {
    //        if (isDisposed)
    //        {
    //            Debug.LogError("isDisposed");
    //            return;
    //        }
    //        if (_target == null)
    //        {
    //            Debug.Log("_target == null");
    //        }
    //        if (this == null)
    //        {
    //            Debug.Log("this == null");
    //        }
    //        try
    //        {
    //            rigidBody.MovePosition(Vector3.Lerp(rigidBody.position, _target.position, Time.deltaTime * 10));
    //        }
    //        catch
    //        {
    //            Debug.LogError("test");
    //        }

    //        //transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * 10);
    //        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cts.Token);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (isDisposed)
    //    {
    //        return;
    //    }

    //    Lean.Pool.LeanPool.Despawn(gameObject);
    //}
}
