using System;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    [Header("Statistiche Base")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

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
    }
    protected virtual void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " ha subito danno. Vita rimanente: " + currentHealth);


        OnHealthChanged?.Invoke(currentHealth, maxHealth);

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

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}