using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider;

    [Header("Settings")]
    [SerializeField] private GameEntity target;
    [SerializeField] private float smoothSpeed = 5f;

    private Coroutine updateCoroutine;
    private float targetHealth;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("HealthUI: Target non assegnato");
            // Tenta di trovare il player se non assegnato, utile per setup rapidi
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.GetComponent<GameEntity>();
        }

        if (target != null) 
        {
            // Iscrizione all'evento
            target.OnHealthChanged += UpdateHealthBar;

            // Inizializzazione immediata senza animazione all'avvio
            healthSlider.maxValue = target.GetMaxHealth();
            healthSlider.value = target.GetCurrentHealth();
        }
        else
        {
            Debug.LogError("HealthUI: Target non assegnato!");
        }
    }
    void OnDestroy()
    {
        // BEST PRACTICE: Disiscriversi sempre dagli eventi per evitare errori quando si cambia scena
        if (target != null)
        {
            target.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;

        // Se c'è già un'animazione in corso, la fermiamo e ne facciamo partire una nuova
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
        updateCoroutine = StartCoroutine(AnimateHealth(currentHealth));
    }

    // Coroutine per l'animazione: sostituisce l'Update() costante
    IEnumerator AnimateHealth(float targetValue)
    {
        while (Mathf.Abs(healthSlider.value - targetValue) > 0.01f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, Time.deltaTime * smoothSpeed);
            yield return null; // Aspetta il frame successivo
        }
        healthSlider.value = targetValue; // Assicura il valore finale esatto
    }
}
