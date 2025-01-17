using MonsterLove.StateMachine;
using UnityEngine;

public class StateDriver
{
    public StateEvent Update;
}


public class UnitObj : UnitBaseObj
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hudRoot;
    private Transform target;
    private StateMachine<UnitStates, StateDriver> fsm;
    private UnitHUD unitHUD;


    protected override void Awake()
    {
        base.Awake();
        hudRoot = transform;
        fsm = new StateMachine<UnitStates, StateDriver>(this);
        fsm.ChangeState(UnitStates.Idle);
        Utill.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameDefine.enemyLayerName));
    }

    public override void InitObj(BattleUnitData _unitData)
    {
        base.InitObj(_unitData);
        agent.speed = 3f;
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
        agent.enabled = true;
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
    }
    private void Idle_Update()
    {
        if (Vector3.Distance(transform.position, target.position) <= GameDefine.EnemyApprochDist)
        {
            fsm.ChangeState(UnitStates.Approach);
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
