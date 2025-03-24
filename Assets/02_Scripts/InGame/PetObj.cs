using UnityEngine;
using UnityEngine.AI;

public class PetObj : MonoBehaviour
{
    public int npcIndex;         // NPC 순번
    public int totalNPCs = 4;    // 전체 NPC 수
    public float radius = 2f;    // 원형 거리

    private float angle;         // 각도 (고정)
    private Vector3 offset;      // 초기 오프셋

    void Start()
    {
        angle = 2 * Mathf.PI * npcIndex / totalNPCs;
        offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    void Update()
    {
        if (GameManager.Instance.PlayerObj.transform == null) return;

        
        // 플레이어 위치 + 고정 오프셋으로 따라감
        transform.position = GameManager.Instance.PlayerObj.transform.position + offset;
    }
}