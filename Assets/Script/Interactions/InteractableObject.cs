using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Dati Standard (Default)")]
    [Tooltip("Dati generici dell'oggetto (Nome, Icona, Frasi di base).")]
    public SO_Interactable interactableData;

    // Indici separati per non confondere i dialoghi normali da quelli della quest
    private int defaultLineIndex = 0;
    private int questLineIndex = 0;

    [Header("Quest Logic")]
    [Tooltip("L'obiettivo che il giocatore DEVE avere attivo affinché questo oggetto faccia avanzare la storia.")]
    public string requiredObjective;

    [Tooltip("Il nuovo obiettivo da impostare dopo l'interazione.")]
    public string updateObjectiveTo;

    [Header("Quest Dialogues (Override)")]
    [Tooltip("Queste frasi verranno dette SOLO se l'obiettivo richiesto è attivo.")]
    [TextArea(3, 10)]
    public string[] questDialogueLines;

    public void Interact()
    {
        // Controllo sicurezza dati base
        if (interactableData == null)
        {
            Debug.LogError("Mancano i dati (SO) su questo oggetto: " + gameObject.name);
            return;
        }

        // 1. VERIFICA STATO MISSIONE
        // Controlliamo se siamo nel momento giusto della storia per attivare la logica speciale
        bool isQuestStepActive = false;

        // Se c'è un ObjectiveManager e abbiamo impostato un requisito...
        if (ObjectiveManager.Instance != null && !string.IsNullOrEmpty(requiredObjective))
        {
            // ...controlliamo se l'obiettivo attuale è quello richiesto
            if (ObjectiveManager.Instance.GetCurrentObjective() == requiredObjective)
            {
                isQuestStepActive = true;
            }
        }
        // Caso speciale: Se requiredObjective è vuoto ma c'è un updateObjectiveTo, 
        // significa che è il primo step di una quest (es. inizio gioco) -> Attiva sempre.
        else if (string.IsNullOrEmpty(requiredObjective) && !string.IsNullOrEmpty(updateObjectiveTo))
        {
            isQuestStepActive = true;
        }

        // 2. SELEZIONE DEL DIALOGO
        string textToShow = "";
        Sprite portraitToShow = interactableData.characterPortrait; // Usiamo sempre la faccia base (o puoi aggiungerne una override se serve)

        // Se siamo nello step della Quest E abbiamo scritto dei dialoghi speciali per la quest...
        if (isQuestStepActive && questDialogueLines.Length > 0)
        {
            // --- CASO QUEST: Usiamo le frasi speciali scritte nell'Inspector ---
            textToShow = questDialogueLines[questLineIndex];

            // Gestione indice Quest (avanza solo nelle frasi quest)
            questLineIndex++;
            if (questLineIndex >= questDialogueLines.Length) questLineIndex = 0;
        }
        else
        {
            // --- CASO NORMALE: Usiamo le frasi generiche del SO ---
            // Questo succede se non è il momento della quest OPPURE se abbiamo già finito questo step
            if (interactableData.dialogueLines.Length > 0)
            {
                textToShow = interactableData.dialogueLines[defaultLineIndex];

                // Gestione indice Default
                defaultLineIndex++;
                if (defaultLineIndex >= interactableData.dialogueLines.Length) defaultLineIndex = 0;
            }
        }

        // 3. MOSTRA DIALOGO
        if (DialogManager.Instance != null && !string.IsNullOrEmpty(textToShow))
        {
            // Nota: Se il tuo DialogManager aggiornato non usa più 'portraitToShow' perché usa la Render Texture,
            // rimuovi il secondo parametro qui sotto.
            DialogManager.Instance.ShowDialog(textToShow, portraitToShow);
        }

        Debug.Log($"[Interazione] {interactableData.displayName}: {textToShow} (Quest Active: {isQuestStepActive})");

        // 4. AGGIORNAMENTO OBIETTIVO (Solo se eravamo nello step attivo e c'è un update da fare)
        if (isQuestStepActive && !string.IsNullOrEmpty(updateObjectiveTo) && ObjectiveManager.Instance != null)
        {
            // Evitiamo di resettare l'obiettivo se è già quello giusto (opzionale, ma pulito)
            if (ObjectiveManager.Instance.GetCurrentObjective() != updateObjectiveTo)
            {
                ObjectiveManager.Instance.SetObjective(updateObjectiveTo);
                Debug.Log($"[Quest] Obiettivo aggiornato a: '{updateObjectiveTo}'");
            }
        }
    }
}