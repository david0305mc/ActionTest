using UnityEngine;

public partial class Util
{
    public static Collider FindClosestInLayer(Vector3 _pos, float _searchRadius, int _targetLayer)
    {
        Collider[] colliders = Physics.OverlapSphere(_pos, _searchRadius, 1 << _targetLayer); 
        Collider closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            float distance = Vector3.Distance(_pos, col.transform.position); 
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = col;
            }
        }

        return closest;
    }
}
