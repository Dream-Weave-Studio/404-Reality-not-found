using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float DamageAmount = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out GameEntity entity))
        {
            entity.TakeDamage(DamageAmount);
        }
    }
}
