using UnityEngine;
using UnityEngine.AI;

public class PetObj : MonoBehaviour
{
    public int npcIndex;         // NPC ����
    public int totalNPCs = 4;    // ��ü NPC ��
    public float radius = 2f;    // ���� �Ÿ�

    private float angle;         // ���� (����)
    private Vector3 offset;      // �ʱ� ������

    void Start()
    {
        angle = 2 * Mathf.PI * npcIndex / totalNPCs;
        offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    void Update()
    {
        if (GameManager.Instance.PlayerObj.transform == null) return;

        
        // �÷��̾� ��ġ + ���� ���������� ����
        transform.position = GameManager.Instance.PlayerObj.transform.position + offset;
    }
}