using UnityEngine;
using UnityEngine.UI;
public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private ProjectileLinearTest projectilePrefab;
    [SerializeField] private BezierArrow bezierArrowPrefab;

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
        var obj = Lean.Pool.LeanPool.Spawn(bezierArrowPrefab, src.transform.position, Quaternion.identity, transform);
        obj.pointA = obj.transform.position;

        obj.pointC = dst.transform.position;
        obj.pointB = (obj.transform.position + dst.transform.position) / 2;
        obj.pointB += Vector3.up * 3;
        //obj.Fire(dst.transform);
    }
}
