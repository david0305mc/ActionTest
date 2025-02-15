using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.AI;

public class StateDriver
{
    public StateEvent Update;
}


public class UnitObj : UnitBaseObj
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hudRoot;
    private Transform target;
    private Vector3 rollingTargetPos;
    private StateMachine<UnitStates, StateDriver> fsm;
    private UnitHUD unitHUD;
    private float targetElapse;
    private float stateElapse;

    protected override void Awake()
    {
        base.Awake();
        hudRoot = transform;
        fsm = new StateMachine<UnitStates, StateDriver>(this);
        fsm.ChangeState(UnitStates.Idle);
        Utill.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameDefine.enemyLayerName));
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    public override void InitObj(BattleUnitData _unitData)
    {
        base.InitObj(_unitData);
        agent.speed = 3f;
        agent.angularSpeed = 1000f;
        //agent.acceleration = 1000f;
        target = GameManager.Instance.PlayerObj.transform;

        unitHUD = Lean.Pool.LeanPool.Spawn(ResourceManager.Instance.UnitHUDPrefab, hudRoot.position, Quaternion.identity, hudRoot).GetComponent<UnitHUD>();
        unitHUD.transform.localPosition = Vector3.up * 2f;
        unitHUD.Init();

        WeaponDetector[] childHandlers = GetComponentsInChildren<WeaponDetector>();
        foreach (var handler in childHandlers)
        {
            handler.SetOnTriggerEnter(_collider =>
            {
                if (weaponEnableFlag)
                {
                    PlayerObj unitObj = _collider.GetComponent<PlayerObj>();
                    if (unitObj != null)
                    {
                        Debug.LogError($"Hit To Player {Count++}");
                        DamageEvent?.Invoke(unitObj);
                    }
                }
            });
        }
    }
    private static int Count = 0; 

    private void Update()
    {
        fsm.Driver.Update.Invoke();
        //agent.Move(scaledMovement);
        //agent.transform.LookAt(playerNavMeshAgent.transform.position + scaledMovement, Vector3.up);
    }

    private void Idle_Enter()
    {
        //Debug.Log("Idle_Enter");
        stateElapse = 0f;
        agent.isStopped = true;
        agent.enabled = false;
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
        animator.SetTrigger("Idle");
    }
    private void Idle_Update()
    {
        stateElapse += Time.deltaTime;
        if (stateElapse < targetElapse)
        {
            return;
        }

        if (Vector3.Distance(transform.position, target.position) <= GameDefine.EnemyApprochDist)
        {
            //fsm.ChangeState(UnitStates.Approach);
            fsm.ChangeState(UnitStates.Rolling);
        }
    }

    private void Rolling_Enter()
    {
        agent.enabled = true;
        agent.isStopped = false;
        animator.SetTrigger("Rolling");
        rollingTargetPos = target.position;
    }

    private void Rolling_Update()
    {
        if (agent.enabled)
        {
            agent.SetDestination(rollingTargetPos);
        }

        var vector = rollingTargetPos - transform.position;

        animator.SetFloat("MoveX", vector.normalized.x);
        animator.SetFloat("MoveZ", vector.normalized.z);

        if (Vector3.Distance(transform.position, rollingTargetPos) < 1f)
        {
            Vector3 dir = (rollingTargetPos - transform.position).normalized;
            rollingTargetPos = rollingTargetPos + dir * 2f;

            if (NavMesh.Raycast(transform.position, rollingTargetPos, out var hit, NavMesh.AllAreas))
            {
                rollingTargetPos = hit.position;
                Debug.Log("Block");
            }
            
            fsm.ChangeState(UnitStates.RollingExtraMove);
            return;
        }
    }

    private void RollingExtraMove_Enter()
    {

        
    }
    private void RollingExtraMove_Update()
    {
        if (agent.enabled)
        {
            agent.SetDestination(rollingTargetPos);
        }

        var vector = rollingTargetPos - transform.position;

        animator.SetFloat("MoveX", vector.normalized.x);
        animator.SetFloat("MoveZ", vector.normalized.z);

        if (Vector3.Distance(transform.position, rollingTargetPos) < 0.5f)
        {
            targetElapse = 2f;
            fsm.ChangeState(UnitStates.Idle);
            return;
        }
    }

    private void Approach_Enter()
    {
        agent.enabled = true;
    }

    private void Approach_Update()
    {
        if (Vector3.Distance(transform.position, target.position) > GameDefine.EnemyApprochDist)
        {
            fsm.ChangeState(UnitStates.Idle);
            return;
        }

        if (Vector3.Distance(transform.position, target.position) < GameDefine.EnemyAttackDist)
        {
            fsm.ChangeState(UnitStates.Attack);
            return;
        }
        if (agent.enabled)
        {
            agent.SetDestination(target.position);
        }
        
        var vector = target.position - transform.position;

        animator.SetFloat("MoveX", vector.normalized.x);
        animator.SetFloat("MoveZ", vector.normalized.z);
    }

    private void Attack_Enter()
    {
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
        attackRemainTime = 0f;
        agent.enabled = false;
    }
    private void Attack_Update()
    {
        UpdateRotate(target.transform);
        attackRemainTime -= Time.deltaTime;
        if (attackRemainTime <= 0)
        {
            attackRemainTime = GameDefine.EnemyAttackCooltime;
            animator.SetTrigger("Attack");
        }
    }

    public void ReactAttack(Vector3 _hitDirection)
    {
        animator.SetTrigger("ReactAttack");
        KnockBack(_hitDirection);

        unitHUD?.SetProgressBar(unitData.hp / (float)3);
    }

    public override void AttackEnd()
    {
        fsm.ChangeState(UnitStates.Idle);
    }

    public override void GetDamaged(AttackMessage _attackMessage)
    {
        Vector3 hitDirection = (transform.position - _attackMessage.attackerObj.transform.position).normalized;
        ReactAttack(hitDirection);
    }
}
