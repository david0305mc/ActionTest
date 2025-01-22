using UnityEngine;

public class MapTestManager : MonoBehaviour
{
    [SerializeField] private MapTestUnitObj unitObjPrefab;
    [SerializeField] private Transform field;
    [SerializeField] private Transform spawnPos;

    public void OnClickSpawnEnemy()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < 10; i++)
        {
            MapTestUnitObj unitObj = Lean.Pool.LeanPool.Spawn(unitObjPrefab, spawnPos, field);
            unitObj.InitObj();
        }
    }
        
}
