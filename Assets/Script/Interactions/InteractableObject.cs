using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Dati")]
    public SO_Interactable interactableData;

    [Header("Setup Gerarchia")]
    public GameObject objectToHide;

    private int defaultLineIndex = 0;
    private int questLineIndex = 0;
    private int variantLineIndex = 0;
    private string lastUsedVariantID = "";

    [Header("Reazioni Modulari")]
    public List<InteractionReaction> reactions = new List<InteractionReaction>();

    [Header("Quest Logic")]
    [Tooltip("ID del sotto-obiettivo che deve essere ATTIVO per rendere interagibile questo oggetto.")]
    public string requiredObjective;

    [Tooltip("ID del sotto-obiettivo da marcare come COMPLETATO quando il player interagisce.")]
    public string completeSubObjectiveID;

    [Tooltip("Evento da sparare al QuestManager (sblocca le Optional in ascolto su questo eventID).")]
    public string fireEventID;

    [Tooltip("Fatto che deve essere già noto (MemoryManager) prima che questo oggetto sia interagibile. Es: seen_calendario")]
    public string prerequisiteFactID;

    [TextArea(3, 10)]
    public string[] questDialogueLines;

    [Header("UI Anchor")]
    public Transform promptAnchor;

    [Header("Spawn Oggetto")]
    [Tooltip("Prefab da istanziare dopo l'interazione. Se vuoto non spawna nulla.")]
    public GameObject spawnPrefab;
    [Tooltip("Punto dove viene spawnato il prefab. Se vuoto usa la posizione di questo oggetto.")]
    public Transform spawnPoint;

    private void OnEnable()
    {
        if (reactions.Count == 0)
            reactions.AddRange(GetComponents<InteractionReaction>());
        StartCoroutine(CheckPersistenceDelayed());
    }

    private System.Collections.IEnumerator CheckPersistenceDelayed()
    {
        yield return null;
        CheckPersistence();
    }

    private void CheckPersistence()
    {
        if (interactableData != null &&
            interactableData.itemToGive != null &&
            interactableData.destroyAfterInteraction)
        {
            if (InventoryManager.Instance != null &&
                InventoryManager.Instance.HasItem(interactableData.itemToGive.itemID))
                SetObjectActive(false);
        }
    }

    public virtual void Interact()
    {
        if (interactableData == null) return;

        // Blocca l'interazione se l'oggetto non e' interagibile in questo momento
        // (es. missione non ancora sbloccata)
        if (!IsInteractable()) return;

        string textToShow = "";
        Sprite portrait = interactableData.characterPortrait;

        bool isQuestStepActive = CheckQuestStatus();

        if (isQuestStepActive && questDialogueLines.Length > 0)
        {
            textToShow = questDialogueLines[questLineIndex];
            questLineIndex = (questLineIndex + 1) % questDialogueLines.Length;

            if (!string.IsNullOrEmpty(completeSubObjectiveID) && QuestManager.Instance != null)
                QuestManager.Instance.CompleteSubObjective(completeSubObjectiveID);

            if (!string.IsNullOrEmpty(fireEventID) && QuestManager.Instance != null)
                QuestManager.Instance.UnlockSubsByEvent(fireEventID);
        }
        else
        {
            if (!string.IsNullOrEmpty(fireEventID) && QuestManager.Instance != null)
                QuestManager.Instance.UnlockSubsByEvent(fireEventID);

            DialogueVariant? activeVariant = GetActiveVariant();
            if (activeVariant.HasValue)
            {
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
            else
            {
                if (interactableData.dialogueLines.Length > 0)
                {
                    textToShow = interactableData.dialogueLines[defaultLineIndex];
                    defaultLineIndex = (defaultLineIndex + 1) % interactableData.dialogueLines.Length;
                }
            }
        }

        foreach (var reaction in reactions)
            reaction.React(this.gameObject);

        if (DialogManager.Instance != null && !string.IsNullOrEmpty(textToShow))
            DialogManager.Instance.ShowDialog(textToShow, portrait);
        else
            Debug.Log("[" + interactableData.displayName + "]: " + textToShow);

        if (interactableData.itemToGive != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(interactableData.itemToGive);
            Debug.Log("Hai raccolto: " + interactableData.itemToGive.itemName);
        }

        if (!string.IsNullOrEmpty(interactableData.factToLearn) && MemoryManager.Instance != null)
            MemoryManager.Instance.SetFact(interactableData.factToLearn);

        // Spawn oggetto (solo se il prefab e' assegnato)
        if (spawnPrefab != null)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
            Instantiate(spawnPrefab, pos, rot);
            Debug.Log("[Spawn] Istanziato: " + spawnPrefab.name);
        }

        if (interactableData.destroyAfterInteraction)
            SetObjectActive(false);
    }

    // Chiamato da InteractionPromptUI e PlayerInteraction prima di mostrare il prompt / eseguire l'interazione
    public virtual bool IsInteractable()
    {
        // Prerequisito fatto: se specificato, deve essere già noto prima di qualsiasi altra cosa
        if (!string.IsNullOrEmpty(prerequisiteFactID))
        {
            if (MemoryManager.Instance == null || !MemoryManager.Instance.CheckFact(prerequisiteFactID))
                return false;
        }

        bool hasQuestLogic = !string.IsNullOrEmpty(requiredObjective) ||
                             !string.IsNullOrEmpty(completeSubObjectiveID);

        // Oggetto CON logica quest
        if (hasQuestLogic)
        {
            if (QuestManager.Instance == null) return false;

            string subToCheck = !string.IsNullOrEmpty(requiredObjective)
                                ? requiredObjective
                                : completeSubObjectiveID;

            // Sub attiva nello stack → prompt visibile
            if (QuestManager.Instance.IsSubActive(subToCheck))
                return true;

            // Sub gia' completata → prompt solo se ha dialoghi post-completamento
            if (QuestManager.Instance.IsSubCompleted(subToCheck))
            {
                if (interactableData == null) return false;
                bool hasDefault = interactableData.dialogueLines != null &&
                                   interactableData.dialogueLines.Length > 0;
                bool hasVariants = interactableData.memoryVariants != null &&
                                   interactableData.memoryVariants.Count > 0;
                return hasDefault || hasVariants;
            }

            // Missione non ancora sbloccata → prompt nascosto
            return false;
        }

        // Oggetto SENZA logica quest (puramente narrativo)
        if (interactableData == null) return false;
        bool hasDialogue = interactableData.dialogueLines != null &&
                           interactableData.dialogueLines.Length > 0;
        bool hasMemory = interactableData.memoryVariants != null &&
                           interactableData.memoryVariants.Count > 0;
        return hasDialogue || hasMemory;
    }

    private bool CheckQuestStatus()
    {
        if (string.IsNullOrEmpty(requiredObjective) && string.IsNullOrEmpty(completeSubObjectiveID))
            return false;

        if (!string.IsNullOrEmpty(requiredObjective))
        {
            if (QuestManager.Instance == null) return false;
            bool active = QuestManager.Instance.IsSubActive(requiredObjective);
            Debug.Log("[Quest Check] " + gameObject.name + " | Richiede: '" + requiredObjective + "' | Attivo: " + active);
            return active;
        }

        return true;
    }

    private void SetObjectActive(bool isActive)
    {
        if (objectToHide != null) objectToHide.SetActive(false);
        else gameObject.SetActive(false);
    }

    private DialogueVariant? GetActiveVariant()
    {
        if (MemoryManager.Instance == null || interactableData.memoryVariants == null) return null;
        foreach (var variant in interactableData.memoryVariants)
            if (MemoryManager.Instance.CheckFact(variant.requiredMemoryID))
                return variant;
        return null;
    }
}