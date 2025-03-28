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
        offset = new Vector3(Mathf.Cos(angle), 1, Mathf.Sin(angle)) * radius;
    }

    void Update()
    {
        if (GameManager.Instance.PlayerObj.transform == null) return;

        
        // 플레이어 위치 + 고정 오프셋으로 따라감
        transform.position = GameManager.Instance.PlayerObj.transform.position + offset;
    }
}


namespace Test
{
    public class BaseObj<T> where T : BaseData
    {
        public T BaseData { get; set; }

    }

    public class AObj : BaseObj<Adata>
    {
        public void TestFun()
        {

        }
    }

    public class BObj : BaseObj<BData> { }

    public class CObj: AObj 
    { 
        public void Fire<T>()  where T : Adata
        { 

        }
    }

    public class BaseData
    {

    }

    public class Adata : BaseData
    {
    }

    public class BData : BaseData
    {
    }


    public class TestClass
    {
        private AObj playerObj;

        private void Func2<T>(BaseObj<T> _param) where T : BaseData
        {
            AttackMessage<Adata>.Create(playerObj);
        }
    }

    public struct AttackMessage<T> where T : BaseData
    {
        public BaseObj<T> attackerObj;

        public static AttackMessage<T> Create(BaseObj<T> _attackerObj)    
        {
                AttackMessage<T> msg = new AttackMessage<T>()
                {
                    attackerObj = _attackerObj,
                };
                return msg;
        }
    
    }
    
}

