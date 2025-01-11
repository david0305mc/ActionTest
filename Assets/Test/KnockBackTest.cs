using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.AI;
public class KnockBackTest : MonoBehaviour
{
    public int knockbackForce = 10;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private NavMeshAgent agent;
    private void Awake()
    {
        agent.enabled = true;
    }

    public void MoveToTarget(GameObject _target)
    {
        UniTask.Create(async () =>
        {
            agent.destination = _target.transform.position;
            await UniTask.Yield();
        });
        
    }
    public async UniTask KnockBack(Vector3 _hitDirection)
    {
        rigidBody.isKinematic = false;
        Vector3 knockbackVector = new Vector3(_hitDirection.x, 0, _hitDirection.z).normalized * knockbackForce;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + knockbackVector;
        rigidBody.AddForce(knockbackVector, ForceMode.Impulse);
    }
}
