using UnityEngine;

using Cysharp.Threading.Tasks;
using System.Threading;
using Cinemachine;
public class ProjectileLinearTest : MonoBehaviour
{
    public Transform target; // B지점 (목표 지점)
    public float speed = 10f; // 화살의 속도
    public float arcHeight = 5f; // 화살이 날아가는 동안의 포물선 높이

    private Vector3 startPoint; // A지점 (출발 지점)
    private Vector3 controlPoint; // 포물선을 만들기 위한 중간 지점
    private float progress = 0f; // 진행도 (0 ~ 1)

    void Start()
    {
        startPoint = transform.position; // A지점 설정
        if (target != null)
        {
            // 중간 제어점을 계산 (A지점과 B지점의 중간 + 높이)
            controlPoint = (startPoint + target.position) / 2;
            controlPoint += Vector3.up * arcHeight;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // 진행도 업데이트
            progress += Time.deltaTime * speed / Vector3.Distance(startPoint, target.position);
            if (progress > 1f) progress = 1f;

            // 베지어 곡선을 따라 화살 위치 계산
            Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, progress);
            Vector3 m2 = Vector3.Lerp(controlPoint, target.position, progress);
            transform.position = Vector3.Lerp(m1, m2, progress);

            // 화살을 목표 방향으로 회전
            transform.LookAt(Vector3.Lerp(controlPoint, target.position, progress));

            // 목표 지점에 도달하면 화살 제거
            if (progress >= 1f)
            {
                OnHitTarget();
            }
        }
    }

    void OnHitTarget()
    {
        // 목표에 도달한 후 행동 정의 (효과, 파괴 등)
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
