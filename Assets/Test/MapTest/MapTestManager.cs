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
        MapTestUnitObj unitObj = Lean.Pool.LeanPool.Spawn(unitObjPrefab, spawnPos, field);
        unitObj.InitObj();
    }
        
}
