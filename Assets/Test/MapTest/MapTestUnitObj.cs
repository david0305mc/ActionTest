using MonsterLove.StateMachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MapTestUnitObj : MonoBehaviour
{
    public enum UnitStates
    {
        Idle,
        RandomMove,
    }
    public AnimationCurve KnockBackCurve;

    [SerializeField] private UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;

    private StateMachine<UnitStates, StateDriver> fsm;
    private float stateElapse;
    private Vector3 randTargetPos;
    private CancellationTokenSource knockbackCts;

    public void InitObj()
    {
        fsm = new StateMachine<UnitStates, StateDriver>(this);
        fsm.ChangeState(UnitStates.Idle);
        agent.updateRotation = false;
    }
    private void Update()
    {
        fsm.Driver.Update.Invoke();
    }
    private void Idle_Enter()
    {
        agent.enabled = true;
        stateElapse = 0f;
        Debug.Log("Idle_Enter");
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
    }
    private void Idle_Update()
    {
        if (stateElapse > 1f)
        {
            fsm.ChangeState(UnitStates.RandomMove);
            return;
        }
        stateElapse += Time.deltaTime;
    }


    private void RandomMove_Enter()
    {
        agent.enabled = true;
        stateElapse = 0;
        if (TryGetValidNavMeshPosition(transform.position, 3f, out Vector3 result))
        {
            randTargetPos = result;
        }
        Debug.Log("RandomMove_Enter");
    }

    private void RandomMove_Update()
    {
        if (agent.enabled == false)
            return;

        if (Vector3.Distance(transform.position, randTargetPos) <= 0.1f)
        {
            agent.SetDestination(randTargetPos);
            UpdateRotate(randTargetPos);
            fsm.ChangeState(UnitStates.Idle);
            return;
        }

        agent.SetDestination(randTargetPos);
        var vector = randTargetPos - transform.position;
        UpdateRotate(randTargetPos);
        animator.SetFloat("MoveX", vector.normalized.x);
        animator.SetFloat("MoveZ", vector.normalized.z);

        //transform.LookAt(randTargetPos, Vector3.up);
        stateElapse += Time.deltaTime;
    }
    private void UpdateRotate(Vector3 _target)
    {
        transform.LookAt(new Vector3(_target.x, transform.position.y, _target.z), Vector3.up);
    }
    public bool TryGetValidNavMeshPosition(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * radius;
            randomPoint.y = center.y; // y ÁÂÇ¥¸¦ °íÁ¤

            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out UnityEngine.AI.NavMeshHit hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        Debug.LogError("TryGetValidNavMeshPosition Failed");
        result = Vector3.zero;
        return false;
    }

    public async UniTask Knockback(Vector3 _attackSrcPos)
    {
        Vector3 knockbackDirection = transform.position - _attackSrcPos;
        knockbackDirection = new Vector3(knockbackDirection.x, 0, knockbackDirection.z).normalized;
        //agent.enabled = false;
        agent.enabled = false;
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + knockbackDirection * 1.5f;

        float elpase = 0f;
        knockbackCts?.Clear();
        knockbackCts = new CancellationTokenSource();

        while (elpase < 1f)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 2, knockbackDirection, out RaycastHit hit, 1f, 1 << LayerMask.NameToLayer("Wall")))
            {
                Debug.Log("Wall");
            }
            else
            {
                float curveValue = KnockBackCurve.Evaluate(elpase);
                rigidBody.MovePosition(Vector3.Lerp(startPos, targetPos, curveValue));
            }
            Debug.DrawRay(transform.position, knockbackDirection * 3f, Color.blue, 1f);
            elpase += Time.deltaTime * 3f;
            await UniTask.Yield(cancellationToken: knockbackCts.Token);
        }

        agent.enabled = true;
        //rigidBody.AddForce(dir * 5f, ForceMode.Impulse);
        //await UniTask.WaitForSeconds(0.3f);

        agent.Warp(transform.position);
        //agent.enabled = true;
    }
}
