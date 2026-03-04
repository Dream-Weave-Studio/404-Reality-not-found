using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest attiva all'avvio")]
    public SO_Quest startingQuest;
    public bool autoStartOnAwake = false;

    [Header("Tutte le quest del gioco (per il save/load)")]
    public List<SO_Quest> allQuests;

    [Header("Events")]
    public UnityEvent<string> OnSubObjectiveUnlocked;
    public UnityEvent<string> OnSubObjectiveCompleted;
    public UnityEvent<SO_Quest> OnMainQuestCompleted;

    private SO_Quest activeQuest;

    // Stack delle missioni attive in ordine di sblocco.
    // L'ultima della lista e' quella attualmente "in cima" (la piu' recente).
    private List<string> activeStack = new List<string>();
    private HashSet<string> completedSubIDs = new HashSet<string>();

    // -------------------------------------------------------

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        if (autoStartOnAwake && startingQuest != null)
            StartQuest(startingQuest);
    }

    // -------------------------------------------------------
    // RUNTIME
    // -------------------------------------------------------

    public void StartQuest(SO_Quest quest)
    {
        activeQuest = quest;
        activeStack.Clear();
        completedSubIDs.Clear();
        Debug.Log("[Quest] Avviata: " + quest.mainDisplayName);
        QuestHUD.Instance?.Refresh(activeQuest, activeStack, completedSubIDs);
    }

    // Chiamato da InteractableObject tramite fireEventID.
    // Sblocca tutte le sub in attesa di quell'evento e le mette in cima allo stack.
    public void UnlockSubsByEvent(string eventID)
    {
        if (activeQuest == null || string.IsNullOrEmpty(eventID)) return;

        bool anyUnlocked = false;
        foreach (var sub in activeQuest.subObjectives)
        {
            if (sub.unlockedByEventID == eventID &&
                !activeStack.Contains(sub.id) &&
                !completedSubIDs.Contains(sub.id))
            {
                activeStack.Add(sub.id);
                Debug.Log("[Quest] Sbloccata: " + sub.id + " (evento: " + eventID + ")");
                OnSubObjectiveUnlocked?.Invoke(sub.id);
                anyUnlocked = true;
            }
        }

        if (anyUnlocked)
            QuestHUD.Instance?.Refresh(activeQuest, activeStack, completedSubIDs);
    }

    // Chiamato da InteractableObject quando il player completa una missione.
    public void CompleteSubObjective(string subID)
    {
        if (activeQuest == null) return;
        if (!activeStack.Contains(subID))
        {
            Debug.LogWarning("[Quest] Tentativo di completare una sub non attiva: " + subID);
            return;
        }

        activeStack.Remove(subID);
        completedSubIDs.Add(subID);
        Debug.Log("[Quest] Completata: " + subID + " | Stack rimanente: " + activeStack.Count);

        OnSubObjectiveCompleted?.Invoke(subID);
        QuestHUD.Instance?.Refresh(activeQuest, activeStack, completedSubIDs);

        // Controlla se tutte le sub della quest sono state completate
        if (AreAllSubsCompleted())
        {
            Debug.Log("[Quest] Tutte le sub completate! Missione principale sbloccata.");
            OnMainQuestCompleted?.Invoke(activeQuest);
        }
    }

    // -------------------------------------------------------
    // CHECK
    // -------------------------------------------------------

    // True se questa sub e' attualmente in cima allo stack (e' la missione attiva ora)
    public bool IsSubActive(string subID)
    {
        if (string.IsNullOrEmpty(subID)) return false;
        return activeStack.Contains(subID);
    }

    // Ritorna l'ID della missione attualmente in cima allo stack (la piu' urgente)
    public string GetCurrentSubID()
    {
        if (activeStack.Count == 0) return "";
        return activeStack[activeStack.Count - 1];
    }

    public bool IsSubCompleted(string subID) => completedSubIDs.Contains(subID);
    public SO_Quest GetActiveQuest() => activeQuest;

    // -------------------------------------------------------
    // SAVE / LOAD
    // -------------------------------------------------------

    public List<string> GetActiveStackForSave() => new List<string>(activeStack);
    public List<string> GetCompletedSubsForSave() => new List<string>(completedSubIDs);

    public void LoadQuestFromSave(string questID, List<string> savedStack, List<string> completedSubs)
    {
        SO_Quest found = allQuests.Find(q => q.questID == questID);
        if (found == null)
        {
            Debug.LogWarning("[QuestManager] questID non trovato: " + questID);
            return;
        }

        activeQuest = found;
        activeStack = new List<string>(savedStack);
        completedSubIDs = new HashSet<string>(completedSubs);

        Debug.Log("[QuestManager] Ripristinata: " + found.mainDisplayName +
                  " | Stack: " + activeStack.Count + " | Completate: " + completedSubIDs.Count);

        QuestHUD.Instance?.Refresh(activeQuest, activeStack, completedSubIDs);

        if (AreAllSubsCompleted()) OnMainQuestCompleted?.Invoke(activeQuest);
    }

    // -------------------------------------------------------
    // PRIVATI
    // -------------------------------------------------------

    private bool AreAllSubsCompleted()
    {
        if (activeQuest == null) return false;
        foreach (var sub in activeQuest.subObjectives)
            if (!completedSubIDs.Contains(sub.id)) return false;
        return true;
    }
}