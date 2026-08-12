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
            mainQuestText.text = ""; // Rimuoviamo la scritta grande

        // Configura il layout group del container per compattare gli elementi
        var layout = subObjectiveContainer.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.spacing = 3f; // Leggermente aumentato per leggibilità con font più grande
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
        }

        foreach (Transform child in subObjectiveContainer)
            Destroy(child.gameObject);

        string currentSubID = activeStack.Count > 0 ? activeStack[activeStack.Count - 1] : "";

        // Dividiamo gli obiettivi in Primari (il telefono) e Secondari (gli altri)
        List<SubObjectiveData> primarySubs = new List<SubObjectiveData>();
        List<SubObjectiveData> secondarySubs = new List<SubObjectiveData>();

        foreach (var sub in quest.subObjectives)
        {
            if (sub.id == "prendi_telefono")
                primarySubs.Add(sub);
            else
                secondarySubs.Add(sub);
        }

        // --- 1. RENDER OBIETTIVO PRIMARIO ---
        CreateHeaderRow("🎯 OBIETTIVO PRIMARIO:");
        foreach (var sub in primarySubs)
        {
            RenderObjectiveRow(sub, currentSubID, activeStack, completed, isPrimary: true);
        }

        // Spazio divisore molto compatto
        CreateDividerRow();

        // --- 2. RENDER OBIETTIVI SECONDARI ---
        CreateHeaderRow("🔍 OBIETTIVI SECONDARI:");
        foreach (var sub in secondarySubs)
        {
            RenderObjectiveRow(sub, currentSubID, activeStack, completed, isPrimary: false);
        }
    }

    private void CreateHeaderRow(string headerText)
    {
        GameObject row = Instantiate(subObjectiveRowPrefab, subObjectiveContainer);
        TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
        if (texts.Length < 2) return;

        ConfigureRowRect(row, height: 22f);
        texts[0].enableAutoSizing = false;
        texts[1].enableAutoSizing = false;
        texts[0].fontSize = 14f; // Ingrandito
        texts[1].fontSize = 14f; // Ingrandito

        texts[0].text = "";
        texts[1].text = $"<color=#FFFF00><b>{headerText}</b></color>";
    }

    private void CreateDividerRow()
    {
        GameObject row = Instantiate(subObjectiveRowPrefab, subObjectiveContainer);
        ConfigureRowRect(row, height: 8f);
        TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
        foreach (var t in texts) t.text = "";
    }

    private void ConfigureRowRect(GameObject row, float height)
    {
        RectTransform rect = row.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }
    }

    private void RenderObjectiveRow(SubObjectiveData sub, string currentSubID, List<string> activeStack, HashSet<string> completed, bool isPrimary)
    {
        bool isDone = completed.Contains(sub.id);
        bool isCurrent = sub.id == currentSubID;
        bool isPaused = activeStack.Contains(sub.id) && !isCurrent;
        bool isLocked = !isDone && !isCurrent && !isPaused;

        GameObject row = Instantiate(subObjectiveRowPrefab, subObjectiveContainer);
        TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
        if (texts.Length < 2) return;

        ConfigureRowRect(row, height: 22f);
        texts[0].enableAutoSizing = false;
        texts[1].enableAutoSizing = false;
        texts[0].fontSize = 14f; // Ingrandito
        texts[1].fontSize = 14f; // Ingrandito

        // Checkbox visiva
        if (isDone)
            texts[0].text = "<color=#00FF88>[\u2713]</color>"; // Spunta universale supportata
        else
            texts[0].text = "<color=#BBBBBB>[  ]</color>"; // Box più chiaro per leggibilità

        // Testo dell'obiettivo
        string displayName = sub.displayName.ToUpper();
        if (isPrimary && isLocked)
        {
            displayName = "?? ????????";
        }

        if (isDone)
        {
            texts[1].text = displayName;
            texts[1].color = new Color(0.6f, 0.6f, 0.6f, 0.9f); // Grigio leggibile
            texts[1].fontStyle = FontStyles.Strikethrough;
        }
        else if (isCurrent)
        {
            texts[1].text = $"{displayName} <color=white><b>(ATTIVO)</b></color>";
            texts[1].color = colorActive;
            texts[1].fontStyle = FontStyles.Bold;
        }
        else if (isPaused)
        {
            texts[1].text = $"{displayName} <color=#FFCC44>(IN CODA)</color>";
            texts[1].color = colorPaused;
            texts[1].fontStyle = FontStyles.Normal;
        }
        else // Locked
        {
            texts[1].text = $"{displayName} <color=#AAAAAA><i>(BLOCCATO)</i></color>";
            texts[1].color = new Color(0.75f, 0.75f, 0.75f, 0.9f); // Lighter gray for readability
            texts[1].fontStyle = FontStyles.Italic;
        }
    }
}