using UnityEngine;
using UnityEngine.UI;

public class ProjectileShadowManager : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private CurvedMissile missilePrefab;

    public Transform startPoint; // 미사일 출발 위치
    public Transform controlPoint; // 곡선을 형성할 중간 지점
    public Transform targetPoint; // 목표 위치

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            CurvedMissile missile = Instantiate(missilePrefab, startPoint.position, Quaternion.identity, transform);
            missile.startPoint = startPoint;
            missile.controlPoint = controlPoint;
            missile.targetPoint = targetPoint;


        });
    }

}
