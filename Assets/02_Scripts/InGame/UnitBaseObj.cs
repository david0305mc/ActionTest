using UnityEngine;
using Cysharp.Threading.Tasks;


public interface IDamageable
{
    public void GetDamaged(AttackMessage _attackData);
}
public class UnitBaseObj : MonoBehaviour, IAnimationEventFowarder, IDamageable
{
    public System.Action<UnitBaseObj> DamageEvent { get; set; }
    [SerializeField] protected UnityEngine.AI.NavMeshAgent agent;

    protected CharacterController characterController;
    protected float attackRemainTime;
    protected Rigidbody rigidBody;
    protected float knockbackForce = 5f; // Knockback Èû
    protected float knockbackDuration = 0.2f; // Knockback Áö¼Ó ½Ã°£
    protected bool weaponEnableFlag;

    public BattleUnitData unitData { get; private set; }

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    public virtual void InitObj(BattleUnitData _unitData)
    { 
        unitData = _unitData;
    }
    protected void UpdateRotate(Transform _target)
    {
        transform.LookAt(_target, Vector3.up);
    }

    public virtual async UniTask KnockBack(Vector3 _hitDirection)
    {
        agent.enabled = false;
        rigidBody.isKinematic = false;
        Vector3 knockbackVector = new Vector3(_hitDirection.x, 0, _hitDirection.z).normalized * knockbackForce;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + knockbackVector;
        rigidBody.AddForce(knockbackVector, ForceMode.Impulse);

        float knockbackTime = Time.time;
        await UniTask.WaitUntil(() =>
        {
            return rigidBody.linearVelocity.magnitude < 0.05f || Time.time > knockbackTime + 0.5f;
        });
        await UniTask.WaitForSeconds(0.25f);
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.isKinematic = true;
        agent.enabled = true;
    }

    public virtual void AttackEnd()
    {
        
    }

    public virtual void EnableWeaponFlag()
    {
        weaponEnableFlag = true;
    }

    public virtual void DisableWeaponFlag()
    {
        weaponEnableFlag = false;
    }

    public virtual void EndDodgeAnimation()
    { 
    
    }

    public virtual void DrawArrow()
    { 
    
    }

    public void OnAnimationEvent(AnimationEventType _eventType)
    {
        switch (_eventType)
        {
            case AnimationEventType.EnableWeaponFlag:
                EnableWeaponFlag();
                break;
            case AnimationEventType.DisableWeaponFlag:
                DisableWeaponFlag();
                break;
            case AnimationEventType.EndAttack:
                AttackEnd();
                break;
            case AnimationEventType.EndDodgeAnimation:
                EndDodgeAnimation();
                break;
            case AnimationEventType.DrawArrow:
                DrawArrow();
                break;
        }
    }

    public virtual void GetDamaged(AttackMessage _attackMessage)
    {
        
    }
}
