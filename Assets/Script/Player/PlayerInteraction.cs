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
    private bool isSubscribed = false;

    #endregion

    #region Ciclo di vita Unity (Enable/Disable)
    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteract += HandleInteraction;
            isSubscribed = true;
        }
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnInteract -= HandleInteraction;
        isSubscribed = false;
    }

    // AGGIUNTA FONDAMENTALE: Controllo costante dello stato
    void Update()
    {
        // Recupera l'iscrizione se OnEnable � andato troppo presto
        if (!isSubscribed && InputManager.Instance != null)
        {
            InputManager.Instance.OnInteract += HandleInteraction;
            isSubscribed = true;
        }
        // Se non abbiamo un target, non c'� nulla da fare
        if (currentTarget == null) return;

        // 1. Controllo Stato: Siamo in Gameplay?
        bool canInteract = GameManager.Instance.currentState == GameManager.GameState.Gameplay
                        || GameManager.Instance.currentState == GameManager.GameState.WakingUp;

        // 2. Gestione UI Dinamica
        InteractableObject obj = currentTarget.GetComponent<InteractableObject>();
        bool canShowPrompt = obj != null && obj.IsInteractable();

        if (canInteract && canShowPrompt && !isPromptActive)
        {
            // Oggetto interagibile e prompt non ancora mostrato -> MOSTRALO
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.ShowPrompt(obj);
                isPromptActive = true;
            }
        }
        else if (canInteract && !canShowPrompt && isPromptActive)
        {
            // Oggetto non piu interagibile (missione cambiata) -> NASCONDILO
            InteractionPromptUI.Instance?.HidePrompt();
            isPromptActive = false;
        }
        else if (!canInteract && isPromptActive)
        {
            // Se NON siamo in gioco (es. parte una cutscene), ma la UI � accesa -> SPEGNILA
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
    /// Se � presente un oggetto attivo che implementa IInteractable, attiva la sua logica.
    /// </summary>

    void HandleInteraction()
    {
        // Blocca se c'� un dialogo in corso
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
            return;

        // Blocca interazione se non � Gameplay
        if (GameManager.Instance != null)
        {
            var state = GameManager.Instance.currentState;
            bool allowed = state == GameManager.GameState.Gameplay
                        || state == GameManager.GameState.WakingUp;
            if (!allowed) return;
        }

        if (currentTarget == null)
        {
            // FALLBACK PER LA SVEGLIA: Se siamo nello stato WakingUp ed il target è nullo,
            // cerchiamo lo SmartSpeaker nella scena per interagire direttamente.
            if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.WakingUp)
            {
                SmartSpeaker speaker = FindObjectOfType<SmartSpeaker>();
                if (speaker != null && speaker.IsInteractable())
                {
                    speaker.Interact();
                    Debug.Log("Interazione di fallback con: " + speaker.name);
                    return;
                }
            }
            return;
        }

        // Interazione generica � pu� diventare un sistema ad eventi
        // Blocca se l oggetto non e interagibile in questo momento
        InteractableObject objCheck = currentTarget.GetComponent<InteractableObject>();
        if (objCheck != null && !objCheck.IsInteractable()) return;

        IInteractable interactable = currentTarget.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
            Debug.Log("Interazione completata con " + currentTarget.name);
        }
    }
    #endregion
}