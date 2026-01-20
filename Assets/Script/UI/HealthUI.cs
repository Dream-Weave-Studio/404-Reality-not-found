using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameEntity target;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetHealth;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("HealthUI: Target non assegnato");
            return;
        }

        target.OnHealthChanged += OnHealthChanged;

        healthSlider.maxValue = target.GetMaxHealth();
        healthSlider.value = target.GetCurrentHealth();
        targetHealth = healthSlider.value;
    }

    void OnHealthChanged(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        targetHealth = currentHealth;
    }

    void Update()
    {
        // Smooth movement
        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            targetHealth,
            Time.deltaTime * smoothSpeed
        );

        if (Input.GetKeyDown(KeyCode.H))
            target.Heal(10);

        if (Input.GetKeyDown(KeyCode.J))
            target.TakeDamage(10);
    }
}
