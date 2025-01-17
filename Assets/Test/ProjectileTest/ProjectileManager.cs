using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private ProjectileLinearTest projectilePrefab;
    [SerializeField] private BezierArrow bezierArrowPrefab;
    [SerializeField] private BezierArrow2 MultiBezierArrowPrefab;

    [SerializeField] private GameObject src;
    [SerializeField] private GameObject dst;
    [SerializeField] private Button button;


    public int missileCount = 5; // 한 번에 발사되는 미사일 개수
    public float missileSpeed = 5f; // 미사일 속도
    public float spawnInterval = 1f; // 발사 주기 (초)

    private void Update()
    {
        Debug.DrawLine(src.transform.position, dst.transform.position, Color.red);
    }
    private void Awake()
    {
        //button.onClick.AddListener(() =>
        //{
        //    Fire();
        //});

        UniTask.Create(async () =>
        {
            while (true)
            {
                FireRadial();
                await UniTask.WaitForSeconds(0.5f);
            }
            
        });
    }
    public void Fire()
    {
        var obj = Lean.Pool.LeanPool.Spawn(bezierArrowPrefab, src.transform.position, Quaternion.identity, transform);

        //var controlPoints = new List<Vector3>();
        //controlPoints.Add(obj.transform.position);
        //controlPoints.Add(((obj.transform.position + dst.transform.position) / 2) + Vector3.up * 5);
        //controlPoints.Add(dst.transform.position);
        //obj.Fire(controlPoints);
        obj.pointA = obj.transform.position;
        obj.pointC = dst.transform.position;
        obj.pointB = (obj.transform.position + dst.transform.position) / 2;
        obj.pointB += Vector3.up * 10;
        //obj.Fire(dst.transform);
    }

    public void FireRadial()
    {
        float angleStep = 360f / missileCount; // 미사일 간 각도
        float currentAngle = 0f;

        for (int i = 0; i < missileCount; i++)
        {
            float angleInRadians = currentAngle + Mathf.Deg2Rad;

            //Vector3 missileDirection = new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sign(angleInRadians));
            //BezierArrow obj = Lean.Pool.LeanPool.Spawn(bezierArrowPrefab, src.transform.position, Quaternion.identity, transform);

            Quaternion fireRotation = Quaternion.Euler(0, currentAngle, 0);

            BezierArrow obj = Lean.Pool.LeanPool.Spawn(bezierArrowPrefab, src.transform.position, fireRotation, transform);

            Vector3 offset = new Vector3(
                Mathf.Sin(angleInRadians) * 10, // X 좌표
                0f, // Y 좌표 (고정)
                Mathf.Cos(angleInRadians) * 10// Z 좌표
            );

            //obj.rigidBody.linearVelocity = obj.transform.forward * 3f;
            //obj.transform.rotation = Quaternion.Euler(missileDirection);
            obj.pointA = obj.transform.position;
            Vector3 dstPos = obj.pointA + obj.transform.forward * 10;
            obj.pointC = offset;
            obj.pointB = (obj.transform.position + obj.pointC) / 2;
            obj.pointB += Vector3.up * 10;

            currentAngle += angleStep;
        }
    }
}
