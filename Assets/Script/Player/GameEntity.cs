using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [Header("Statistiche Base")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " ha subito danno. Vita rimanente: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " è morto/a.");
        gameObject.SetActive(false);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}