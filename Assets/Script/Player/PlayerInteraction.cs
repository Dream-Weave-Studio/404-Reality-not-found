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

    // Serve per evitare di chiamare la UI ogni singolo frame
    private bool isPromptActive = false;
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

    // AGGIUNTA FONDAMENTALE: Controllo costante dello stato
    void Update()
    {
        // Se non abbiamo un target, non c'è nulla da fare
        if (currentTarget == null) return;

        // 1. Controllo Stato: Siamo in Gameplay?
        bool canInteract = GameManager.Instance.currentState == GameManager.GameState.Gameplay;

        // 2. Gestione UI Dinamica
        if (canInteract && !isPromptActive)
        {
            // Se siamo in gioco, ho un target, ma la UI è spenta -> ACCENDILA
            // (Succede quando finisce l'intro e sei già sopra la sveglia)
            InteractableObject objScript = currentTarget.GetComponent<InteractableObject>();
            if (objScript != null && InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.ShowPrompt(objScript);
                isPromptActive = true;
            }
        }
        else if (!canInteract && isPromptActive)
        {
            // Se NON siamo in gioco (es. parte una cutscene), ma la UI è accesa -> SPEGNILA
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.HidePrompt();
                isPromptActive = false;
            }
        }
    }
    #endregion

    #region Trigger di rilevamento oggetti interagibili
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentTarget = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == currentTarget)
        {
            currentTarget = null;

            isPromptActive = false; // Reset stato locale

            // Nascondi UI
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.HidePrompt();
            }
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
