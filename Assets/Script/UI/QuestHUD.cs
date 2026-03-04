using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestHUD : MonoBehaviour
{
    public static QuestHUD Instance;

    [Header("Testo Missione Principale")]
    public TMP_Text mainQuestText;

    [Header("Lista Sotto-obiettivi")]
    public Transform subObjectiveContainer;
    public GameObject subObjectiveRowPrefab;

    [Header("Colori")]
    public Color colorActive = Color.white;
    public Color colorPaused = new Color(1f, 0.8f, 0.3f, 0.7f); // giallo tenue = in attesa
    public Color colorCompleted = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // activeStack: lista ordinata, l'ultimo e' quello in cima (attivo ora)
    public void Refresh(SO_Quest quest, List<string> activeStack, HashSet<string> completed)
    {
        if (quest == null) return;

        if (mainQuestText != null)
            mainQuestText.text = quest.mainDisplayName.ToUpper();

        foreach (Transform child in subObjectiveContainer)
            Destroy(child.gameObject);

        string currentSubID = activeStack.Count > 0 ? activeStack[activeStack.Count - 1] : "";

        foreach (var sub in quest.subObjectives)
        {
            bool isDone = completed.Contains(sub.id);
            bool isCurrent = sub.id == currentSubID;
            bool isPaused = activeStack.Contains(sub.id) && !isCurrent;

            // Mostra solo: missione attiva ora, missioni in pausa nello stack, completate
            // Quelle non ancora sbloccate: nascoste
            if (!isDone && !isCurrent && !isPaused) continue;

            GameObject row = Instantiate(subObjectiveRowPrefab, subObjectiveContainer);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            if (texts.Length < 2) continue;

            // Icona
            if (isDone)
                texts[0].text = "<color=#00FF88>\u2713</color>";     // checkmark verde
            else if (isCurrent)
                texts[0].text = "<color=white>\u25CF</color>";       // pallino pieno bianco
            else
                texts[0].text = "<color=#FFCC44>\u25D4</color>";     // mezzopieno giallo = in pausa

            // Testo
            texts[1].text = sub.displayName.ToUpper();

            if (isDone)
            {
                texts[1].color = colorCompleted;
                texts[1].fontStyle = FontStyles.Strikethrough;
            }
            else if (isCurrent)
            {
                texts[1].color = colorActive;
                texts[1].fontStyle = FontStyles.Bold;
            }
            else // paused
            {
                texts[1].color = colorPaused;
                texts[1].fontStyle = FontStyles.Normal;
            }
        }
    }
}