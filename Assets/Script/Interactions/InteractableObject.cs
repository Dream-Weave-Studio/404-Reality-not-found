using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Dati")]
    public SO_Interactable interactableData;

    [Header("Setup Gerarchia")]
    [Tooltip("Se questo script è su un figlio, trascina qui il Padre da nascondere. Se lo lasci vuoto, nasconderà se stesso.")]
    public GameObject objectToHide;

    // Indici per ciclare i dialoghi
    private int defaultLineIndex = 0;
    private int questLineIndex = 0;
    private int variantLineIndex = 0; // Nuovo indice per le varianti

    // Teniamo traccia dell'ultima variante usata per non resettare l'indice a caso
    private string lastUsedVariantID = "";

    [Header("Quest Logic (Priorità Alta)")]
    public string requiredObjective;
    public string updateObjectiveTo;

    [TextArea(3, 10)]
    public string[] questDialogueLines;


    private void OnEnable()
    {
        StartCoroutine(CheckPersistenceDelayed());
    }


    private System.Collections.IEnumerator CheckPersistenceDelayed()
    {
        // Aspetta 1 frame. Questo dà tempo al SaveManager di eseguire OnSceneLoaded
        // e riempire l'inventario PRIMA che l'oggetto controlli se deve sparire.
        yield return null;

        CheckPersistence();
    }

    // Controlla se l'oggetto deve esistere o no in base ai salvataggi
    private void CheckPersistence()
    {
        // Se questo oggetto dà un item, ed è impostato per distruggersi...
        if (interactableData != null &&
            interactableData.itemToGive != null &&
            interactableData.destroyAfterInteraction)
        {
            // ...controlliamo se il giocatore ce l'ha già in tasca.
            if (InventoryManager.Instance != null &&
                InventoryManager.Instance.HasItem(interactableData.itemToGive.itemID))
            {
                // Se ce l'ha, ci disattiviamo subito.
                SetObjectActive(false);
            }
        }
    }

    public virtual void Interact()
    {
        if (interactableData == null) return;

        string textToShow = "";
        // Default: usiamo il ritratto del SO. 
        // Se volessi cambiarlo dinamicamente, lo faresti qui.
        Sprite portrait = interactableData.characterPortrait;

        // ---------------------------------------------------------
        // FASE 1: CONTROLLO QUEST (Priorità Massima)
        // ---------------------------------------------------------
        bool isQuestStepActive = CheckQuestStatus();

        if (isQuestStepActive && questDialogueLines.Length > 0)
        {
            // Siamo in una quest critica: usiamo le linee scritte nell'Inspector
            textToShow = questDialogueLines[questLineIndex];

            // Avanzamento indice
            questLineIndex = (questLineIndex + 1) % questDialogueLines.Length;

            // Aggiorna Obiettivo (Se necessario)
            if (!string.IsNullOrEmpty(updateObjectiveTo) && ObjectiveManager.Instance != null)
            {
                if (ObjectiveManager.Instance.GetCurrentObjective() != updateObjectiveTo)
                {
                    ObjectiveManager.Instance.SetObjective(updateObjectiveTo);
                }
            }
        }
        // ---------------------------------------------------------
        // FASE 2: CONTROLLO MEMORIA (Priorità Media)
        // ---------------------------------------------------------
        else
        {
            // Cerchiamo se c'è una variante attiva (es. Ho visto il calendario?)
            DialogueVariant? activeVariant = GetActiveVariant();

            if (activeVariant.HasValue)
            {
                // Trovata variante! Resettiamo indice se è cambiata la variante
                if (lastUsedVariantID != activeVariant.Value.requiredMemoryID)
                {
                    variantLineIndex = 0;
                    lastUsedVariantID = activeVariant.Value.requiredMemoryID;
                }

                string[] lines = activeVariant.Value.alternateLines;
                if (lines.Length > 0)
                {
                    textToShow = lines[variantLineIndex];
                    variantLineIndex = (variantLineIndex + 1) % lines.Length;
                }
            }
            // ---------------------------------------------------------
            // FASE 3: DEFAULT (Priorità Bassa)
            // ---------------------------------------------------------
            else
            {
                if (interactableData.dialogueLines.Length > 0)
                {
                    textToShow = interactableData.dialogueLines[defaultLineIndex];
                    defaultLineIndex = (defaultLineIndex + 1) % interactableData.dialogueLines.Length;
                }
            }
        }

        // ---------------------------------------------------------
        // FASE 4: OUTPUT & APPRENDIMENTO
        // ---------------------------------------------------------

        // Invia alla UI
        if (DialogManager.Instance != null && !string.IsNullOrEmpty(textToShow))
        {
            DialogManager.Instance.ShowDialog(textToShow, portrait);
        }
        else
        {
            // Fallback se non c'è il manager
            Debug.Log($"[{interactableData.displayName}]: {textToShow}");
        }

        // Raccolta oggetti (se applicabile)
        if (interactableData.itemToGive != null && InventoryManager.Instance != null)
        {
            // 1. Aggiungi all'inventario
            InventoryManager.Instance.AddItem(interactableData.itemToGive);

            // 2. Feedback (Opzionale: suono o messaggio a schermo)
            Debug.Log($"Hai raccolto: {interactableData.itemToGive.itemName}");
        }

        // Impara il fatto (se questo oggetto insegna qualcosa) [cite: 9]
        // Lo facciamo alla fine, così la PRIMA volta che clicchi leggi il default, 
        // e solo dopo aver letto hai "imparato".
        if (!string.IsNullOrEmpty(interactableData.factToLearn) && MemoryManager.Instance != null)
        {
            MemoryManager.Instance.SetFact(interactableData.factToLearn);
        }

        // RIMOZIONE
        // Se è un oggetto da raccogliere (come il Telefono), deve sparire dalla scena
        if (interactableData.destroyAfterInteraction)
        {
            SetObjectActive(false);
        }
    }

    // Helper per verificare lo stato della Quest (preso dal tuo script originale)
    private bool CheckQuestStatus()
    {
        if (ObjectiveManager.Instance == null) return false;

        // --- DEBUG ---
        string current = ObjectiveManager.Instance.GetCurrentObjective();
        Debug.Log($"[DEBUG QUEST] Oggetto: {gameObject.name} | Richiesto: '{requiredObjective}' | Attuale: '{current}'");

        // Caso 1: C'è un obiettivo richiesto specifico
        if (!string.IsNullOrEmpty(requiredObjective))
        {
            return string.Equals(
                ObjectiveManager.Instance.GetCurrentObjective().Trim(),
                requiredObjective.Trim(),
                System.StringComparison.OrdinalIgnoreCase
                );
        }
        // Caso 2: È uno start point (nessun requisito ma setta un nuovo obiettivo)
        else if (!string.IsNullOrEmpty(updateObjectiveTo))
        {
            return true;
        }

        return false;
    }

    // Helper per gestire la disattivazione gerarchica (Padre vs Figlio)
    private void SetObjectActive(bool isActive)
    {
        // Caso A: Hai assegnato manualmente un padre
        if (objectToHide != null)
        {
            objectToHide.SetActive(false);
        }
        // Caso B: Non hai assegnato nulla, spegne se stesso
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Helper per cercare la variante corretta
    private DialogueVariant? GetActiveVariant()
    {
        if (MemoryManager.Instance == null || interactableData.memoryVariants == null) return null;

        // Cerchiamo nella lista delle varianti se ce n'è una soddisfatta
        foreach (var variant in interactableData.memoryVariants)
        {
            if (MemoryManager.Instance.CheckFact(variant.requiredMemoryID))
            {
                return variant; // Ritorna la prima variante valida trovata
            }
        }
        return null;
    }

}