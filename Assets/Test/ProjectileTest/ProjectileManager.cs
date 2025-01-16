using UnityEngine;
using UnityEngine.UI;
public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private ProjectileLinearTest projectilePrefab;

    [SerializeField] private GameObject src;
    [SerializeField] private GameObject dst;
    [SerializeField] private Button button;


    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            Fire();
        });
    }
    public void Fire()
    {
        var obj = Lean.Pool.LeanPool.Spawn(projectilePrefab, src.transform.position, Quaternion.identity, transform);
        obj.target = dst.transform;
        //obj.Fire(dst.transform);
    }
}
