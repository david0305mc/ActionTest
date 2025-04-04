using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class MapTestPlayerObj : MonoBehaviour
{
    public FloatingJoystick Joystick;
    public Animator playerAnimator;
    public NavMeshAgent playerNavMeshAgent;

    public Vector2 MovementAmount;
    private Finger MovementFinger;
    private Vector2 JoystickSize;

    void Start()
    {
        JoystickSize = new Vector2(Joystick.JoyStickObj.sizeDelta.x, Joystick.JoyStickObj.sizeDelta.y);
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
        if (MovementFinger == null && touchedFinger.screenPosition.x <= Screen.width)
        {
            MovementFinger = touchedFinger;
            MovementAmount = Vector2.zero;
            Joystick.gameObject.SetActive(true);
            Joystick.JoyStickObj.sizeDelta = JoystickSize;
            Joystick.JoyStickObj.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
        }
    }


    private void HandleLoseFinger(Finger lostFinger)
    {
        if (lostFinger == MovementFinger)
        {
            MovementFinger = null;
            Joystick.Knob.anchoredPosition = Vector2.zero;
            Joystick.gameObject.SetActive(false);
            MovementAmount = Vector2.zero;
        }
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
        if (MovementAmount != Vector2.zero)
        {
            Vector3 scaledMovement = playerNavMeshAgent.speed * Time.deltaTime * new Vector3(MovementAmount.x, 0, MovementAmount.y);

            playerNavMeshAgent.Move(scaledMovement);

            playerNavMeshAgent.transform.LookAt(playerNavMeshAgent.transform.position + scaledMovement, Vector3.up);

            playerAnimator.SetFloat("MoveX", MovementAmount.x);
            playerAnimator.SetFloat("MoveZ", MovementAmount.y);
        }
        else
        {
            playerNavMeshAgent.Move(Vector3.zero);
            playerAnimator.SetFloat("MoveX", MovementAmount.x);
            playerAnimator.SetFloat("MoveZ", MovementAmount.y);
            //transform.LookAt(enemyObj.transform);
            UpdateAttack();
        }

    }

    private void UpdateAttack()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            playerAnimator.SetTrigger("Attack");
        }
    }

    public void Attack()
    {
        var collder = Physics.OverlapSphere(transform.position, 7f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach(var item in collder)
        {
            var enemyObj = item.transform.GetComponent<MapTestUnitObj>();
            if (enemyObj != null)
            {
                enemyObj.Knockback(transform.position);
            }
        }
    }
}
