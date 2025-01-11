using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ProjectileLinear : MonoBehaviour
{
    public System.Action<UnitBaseObj> DamageEvent { get; set; }

    [SerializeField] private Rigidbody rigidBody;
    private CancellationTokenSource cts;
    private bool isDisposed;
    public async UniTask Fire(UnitBaseObj _target)
    {
        isDisposed = false;
        cts = new CancellationTokenSource();
        Vector3 startPos = transform.position;
        while (true)
        {
            if (isDisposed)
            {
                Debug.LogError("isDisposed");
                return;
            }
            if(_target == null)
            {
                Debug.Log("_target == null");
            }
            if (this == null)
            {
                Debug.Log("this == null");
            }
            try
            {
                rigidBody.MovePosition(Vector3.Lerp(rigidBody.position, _target.transform.position, Time.deltaTime * 10));
            }
            catch
            {
                Debug.LogError("test");
            }
            
            //transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * 10);
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken:cts.Token);
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDisposed)
        {
            return;
        }
        
        var targetObj = other.GetComponent<UnitBaseObj>();
        if (targetObj != null)
        {
            DamageEvent?.Invoke(targetObj);
        }
    }

    public void Dispose()
    {
        isDisposed = true;
        cts?.Clear();
        Lean.Pool.LeanPool.Despawn(gameObject);
    }
}
