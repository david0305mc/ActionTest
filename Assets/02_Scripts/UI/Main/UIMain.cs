using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMain : SingletonMono<UIMain>
{
    [SerializeField] private Button spawnEnemeyBtn;
    [SerializeField] private Button dodgeBtn;
    [SerializeField] private Button toggleMeleeBtn;
    public FloatingJoystick Joystick;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        spawnEnemeyBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.SpawnEnemy();
        });
        dodgeBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.DodgePlayer();
        });
        toggleMeleeBtn.onClick.AddListener(() => {
            GameManager.Instance.PlayerObj.TogleMeleeAttack();
        });
    }
    public void Broadcast(string funcName)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            gameObject.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
        }
    }
}
