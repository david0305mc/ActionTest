using UnityEngine;
using UnityEngine.UI;
public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private ProjectileLinearTest projectilePrefab;
    [SerializeField] private BezierArrow bezierArrowPrefab;
    [SerializeField] private BezierArrow2 MultiBezierArrowPrefab;

    [SerializeField] private GameObject src;
    [SerializeField] private GameObject dst;
    [SerializeField] private Button button;


    private void Update()
    {
        Debug.DrawLine(src.transform.position, dst.transform.position, Color.red);
    }
    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            Fire();
        });
    }
    public void Fire()
    {
        var obj = Lean.Pool.LeanPool.Spawn(MultiBezierArrowPrefab, src.transform.position, Quaternion.identity, transform);


        obj.Fire(((obj.transform.position + dst.transform.position) / 2) + Vector3.up * 5, dst.transform.position);
        //obj.pointA = obj.transform.position;

        //obj.pointC = dst.transform.position;
        //obj.pointB = (obj.transform.position + dst.transform.position) / 2;
        //obj.pointB += Vector3.up * 10;
        //obj.Fire(dst.transform);
    }
}
