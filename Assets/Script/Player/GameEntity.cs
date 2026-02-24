using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [Header("Statistiche Base")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    // Riferimento centralizzato all'Animator
    protected Animator animator;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // Ottiene l'Animator se presente sullo stesso GameObject
        animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        LogDamage(currentHealth);

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

    // Debug Helpers - Compilati solo in Editor
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void LogDamage(float remainingHealth)
    {
        Debug.Log($"{gameObject.name} ha subito danno. Vita rimanente: {remainingHealth}", this);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void LogDeath()
    {
        Debug.Log($"{gameObject.name} è morto/a.", this);
    }
}