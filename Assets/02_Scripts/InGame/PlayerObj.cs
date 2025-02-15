using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;
public class PlayerObj : UnitBaseObj
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform hudRoot;
    public float SeachDist = 2f;
    [HideInInspector] public FloatingJoystick Joystick;
    
    private Vector2 MovementAmount;
    private Finger MovementFinger;
    private Vector2 JoystickSize;

    //private UnitObj targetObj;
    private BattleUnitData targetData;
    
    private float dodgeElapse;
    private StateMachine<UnitStates, StateDriver> fsm;

    public System.Action<UnitBaseObj> ProjectileFireEvent { get; set; }
    private bool isMeleeAttack;
    private UnitHUD unitHUD;

    protected override void Awake()
    {
        base.Awake();
        fsm = new StateMachine<UnitStates, StateDriver>(this);
        fsm.ChangeState(UnitStates.Idle);
        agent.speed = 3f;
        isMeleeAttack = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    void Start()
    {
        hudRoot = transform;
        Joystick = UIMain.Instance.Joystick;
        JoystickSize = new Vector2(Joystick.JoyStickObj.sizeDelta.x, Joystick.JoyStickObj.sizeDelta.y);
        
        WeaponDetector[] childHandlers = GetComponentsInChildren<WeaponDetector>();
        foreach (var handler in childHandlers)
        {
            handler.SetOnTriggerEnter(_collider => {
                if (weaponEnableFlag)
                {
                    UnitObj unitObj = _collider.GetComponent<UnitObj>();
                    if (unitObj != null)
                    {
                        DamageEvent?.Invoke(unitObj);
                    }
                }
            });
        }
        unitHUD = Lean.Pool.LeanPool.Spawn(ResourceManager.Instance.UnitHUDPrefab, hudRoot.position, Quaternion.identity, hudRoot).GetComponent<UnitHUD>();
        unitHUD.transform.localPosition = Vector3.up * 2f;
        unitHUD.Init();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();

    }

    public override void InitObj(BattleUnitData _unitData)
    {
        base.InitObj(_unitData);
        Utill.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameDefine.playerLayerName));
    }
    private void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == MovementFinger)
        {
            Vector2 knobPosition;
            float maxMovement = JoystickSize.x / 2f;
            ETouch.Touch currentTouch = movedFinger.currentTouch;

            if (Vector2.Distance(
                currentTouch.screenPosition,
                Joystick.JoyStickObj.anchoredPosition
            ) > maxMovement)
            {
                knobPosition = (
                                   currentTouch.screenPosition - Joystick.JoyStickObj.anchoredPosition
                               ).normalized
                               * maxMovement;
            }
            else
            {
                knobPosition = currentTouch.screenPosition - Joystick.JoyStickObj.anchoredPosition;
            }

            Joystick.Knob.anchoredPosition = knobPosition;
            MovementAmount = knobPosition / maxMovement;
        }
    }

    private void HandleFingerDown(Finger touchedFinger)
    {
        if (RaycastUtilities.PointerIsOverUI(Input.mousePosition))
            return;

        if (fsm.State == UnitStates.Dodge)
            return;
        
        if (MovementFinger == null && touchedFinger.screenPosition.x <= Screen.width)
        {
            MovementFinger = touchedFinger;
            MovementAmount = Vector2.zero;
            Joystick.gameObject.SetActive(true);
            Joystick.JoyStickObj.sizeDelta = JoystickSize;
            Joystick.JoyStickObj.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);

            animator.ResetTrigger();
            animator.SetTrigger("Idle");
            Debug.Log("HandleFingerDown");
        }
    }


    private void HandleLoseFinger(Finger lostFinger)
    {
        MovementFinger = null;
        Joystick.Knob.anchoredPosition = Vector2.zero;
        Joystick.gameObject.SetActive(false);
        MovementAmount = Vector2.zero;

        animator.SetFloat("MoveX", MovementAmount.x);
        animator.SetFloat("MoveZ", MovementAmount.y);
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if (startPosition.x < JoystickSize.x / 2)
        {
            startPosition.x = JoystickSize.x / 2;
        }

        if (startPosition.y < JoystickSize.y / 2)
        {
            startPosition.y = JoystickSize.y / 2;
        }
        else if (startPosition.y > Screen.height - JoystickSize.y / 2)
        {
            startPosition.y = Screen.height - JoystickSize.y / 2;
        }

        return startPosition;
    }
    void Update()
    {
        fsm.Driver.Update.Invoke();
    }

    public void Dodge()
    {
        fsm.ChangeState(UnitStates.Dodge);
    }

    public void TogleMeleeAttack()
    {
        if (isMeleeAttack)
        {
            isMeleeAttack = false;
            SeachDist = 20f;
        }
        else
        {
            isMeleeAttack = true;
            SeachDist = 2f;
        }
    }

    private void Idle_Enter()
    {
        agent.enabled = true;
    }
    private void Idle_Update()
    {
        if (MovementAmount != Vector2.zero)
        {
            Vector3 scaledMovement = agent.speed * Time.deltaTime * new Vector3(MovementAmount.x, 0, MovementAmount.y);

            agent.Move(scaledMovement);
            agent.transform.LookAt(agent.transform.position + scaledMovement, Vector3.up);

            animator.SetFloat("MoveX", MovementAmount.x);
            animator.SetFloat("MoveZ", MovementAmount.y);
        }
        else
        {
            FindTarget();
            if (targetData != default)
            {
                fsm.ChangeState(UnitStates.Attack);
            }
        }
        //UpdateAttackState();
    }

    private void Dodge_Enter()
    {
        Debug.Log("Dodge_Enter");
        animator.ResetTrigger();
        animator.SetTrigger("Dodge");
        dodgeElapse = 0f;
        //agent.enabled = false;
    }

    private void Dodge_Update()
    { 
        dodgeElapse += Time.deltaTime;
        Vector3 scaledMovement = agent.speed * Time.deltaTime * agent.transform.forward;
        agent.Move(scaledMovement);
        
        if (dodgeElapse >= 0.8f)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            Debug.Log("Idle 03");
            fsm.ChangeState(UnitStates.Idle);
        }
    }

    private void Attack_Enter()
    {
        Debug.Log("Attack_Enter");
        if (targetData.isDead)
        {
            Debug.LogError("targetData.isDead");
        }    
        var targetObj = GameManager.Instance.GetEnemyObj(targetData.uid);
        UpdateRotate(targetObj.transform);
        attackRemainTime = 0;
    }
    private void Attack_Update()
    {
        if (MovementAmount != Vector2.zero)
        {
            Debug.Log("Idle 02");
            fsm.ChangeState(UnitStates.Idle);
            return;
        }

        if (targetData.isDead)
        {
            fsm.ChangeState(UnitStates.Idle);
            return;
        }

        attackRemainTime -= Time.deltaTime;
        if (attackRemainTime <= 0)
        {
            attackRemainTime = GameDefine.PlayerAttackCooltime;
            if (isMeleeAttack)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("DrawArrow");
            }
        }

        var targetObj = GameManager.Instance.GetEnemyObj(targetData.uid);
        if (Vector3.Distance(transform.position, targetObj.transform.position) > SeachDist)
        {
            Debug.Log("Idle 01");
            fsm.ChangeState(UnitStates.Idle);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, SeachDist);
    }

    private void FindTarget()
    {
        //if (targetObj != null && Vector3.Distance(transform.position, targetObj.transform.position) <= SeachDist)
        //{
        //    return;
        //}

        UnitObj unitObj = Utill.FindTargetEnemy(transform.position, SeachDist, LayerMask.NameToLayer(GameDefine.enemyLayerName));
        if (unitObj != null)
        {
            targetData = unitObj.unitData;
        }
        else
        {
            targetData = default;
        }
    }

    public override void DrawArrow()
    {
        base.DrawArrow();
        try
        {
            if (targetData.isDead)
            { 
                fsm.ChangeState(UnitStates.Idle);
                return;
            }

            var targetObj = GameManager.Instance.GetEnemyObj(targetData.uid);
            ProjectileFireEvent?.Invoke(targetObj);
        }
        catch
        {
            Debug.LogError("targetObj not found");
        }
        
    }

    public override UniTask KnockBack(Vector3 _hitDirection)
    {
        //fsm.ChangeState(UnitStates.KnockBack);
        return base.KnockBack(_hitDirection);

    }
    public override void GetDamaged(AttackMessage _attackMessage)
    {
        Vector3 hitDirection = (transform.position - _attackMessage.attackerObj.transform.position).normalized;
        ReactAttack(hitDirection);
        Debug.Log("Player Obj GetDamaged");
    }
    public void ReactAttack(Vector3 _hitDirection)
    {
        animator.SetTrigger("ReactAttack");
        //KnockBack(_hitDirection);

        unitHUD?.SetProgressBar((float)unitData.hp / unitData.maxHP);
    }
    public Transform GetCharacterAniTrn()
    {
        return animator.transform;
    }
}
