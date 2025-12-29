using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
/// Gestisce le interazioni del giocatore con oggetti nella scena.
/// Rileva quando il giocatore entra in contatto con oggetti interagibili
/// e attiva la logica corrispondente quando riceve il comando di interazione.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    #region Riferimenti e stato locale

    /// <summary>
    /// Riferimento al collider dell'oggetto attualmente interagibile.
    /// Se null non c'è nessun oggetto con cui interagire.
    /// </summary>
    private Collider currentTarget;
    #endregion

    #region Ciclo di vita Unity (Enable/Disable)
    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnInteract += HandleInteraction;
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnInteract -= HandleInteraction;
    }
    #endregion

    #region Trigger di rilevamento oggetti interagibili
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentTarget = other;
            Debug.Log("Oggetto interattivo rilevato: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == currentTarget)
        {
            currentTarget = null;
        }
    }
    #endregion

    #region Logica di interazione

    /// <summary>
    /// Viene chiamato quando il giocatore preme il tasto di interazione.
    /// Se è presente un oggetto attivo che implementa IInteractable, attiva la sua logica.
    /// </summary>

    void HandleInteraction()
    {
        // AGGIUNTA: Blocca interazione se non è Gameplay
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Gameplay)
            return;

        if (currentTarget == null)
            return;

        // Interazione generica — può diventare un sistema ad eventi
        IInteractable interactable = currentTarget.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
            Debug.Log("Interazione completata con " + currentTarget.name);
        }
    }
    #endregion
}
