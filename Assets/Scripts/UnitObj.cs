using UnityEngine;


public interface Damagable
{
    public void Damaged();
}


public class UnitObj : MonoBehaviour, Damagable
{
    [SerializeField] private Animator animator;
    public void Damaged()
    {
        Debug.Log("GetDamage");
        animator.SetTrigger("GetDamage");
    }


}
