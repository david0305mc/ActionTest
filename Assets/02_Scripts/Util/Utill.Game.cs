using UnityEngine;

public partial class Util
{
    public static UnitObj FindTargetEnemy(Vector3 _pos, float _searchRadius, int _targetLayer)
    {
        Collider[] colliders = Physics.OverlapSphere(_pos, _searchRadius, 1 << _targetLayer);
        UnitObj closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            var enemyObj = col.gameObject.GetComponent<UnitObj>();
            
            if (enemyObj == null || enemyObj.unitData == null)
                continue;

            if (enemyObj.unitData.isDead)
                continue;

            float distance = Vector3.Distance(_pos, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemyObj;
            }
        }

        return closest;
    }
}
