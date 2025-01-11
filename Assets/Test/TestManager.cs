using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private Button knockbackButton;
    [SerializeField] private KnockBackTest knockBackTest;
    [SerializeField] private GameObject targetObj;

    private void Awake()
    {
        testButton.onClick.AddListener(() => {
            
            knockBackTest.MoveToTarget(targetObj);
        });

        knockbackButton.onClick.AddListener(() =>
        {
            Vector3 direction = (knockBackTest.transform.position - targetObj.transform.position).normalized;
            knockBackTest.KnockBack(direction);
        });
    }
}
