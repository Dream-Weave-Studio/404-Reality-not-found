using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float DamageAmount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<GameEntity>().TakeDamage(DamageAmount);
        }
    }
}
