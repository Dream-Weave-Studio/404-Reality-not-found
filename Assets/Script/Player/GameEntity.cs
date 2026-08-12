using System;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [Header("Statistiche Base")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    // Riferimento centralizzato all'Animator
    protected Animator animator;

    // Evento per notificare la UI
    public event Action<float, float> OnHealthChanged;

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        // Ottiene l'Animator se presente sullo stesso GameObject
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        LogDamage(currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        LogDeath();
        gameObject.SetActive(false);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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