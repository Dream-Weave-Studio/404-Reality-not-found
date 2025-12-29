using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI References")]
    public GameObject objectivePanel;
    public TMP_Text objectiveText;

    [Header("Settings")]
    public float fadeDuration = 0.5f; // Durata dell'effetto lampeggio

    // DATI: La stringa "pura" che serve al SaveSystem e alla logica
    private string currentObjectiveID = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // All'inizio nascondiamo l'obiettivo se non ce n'è uno
        if (string.IsNullOrEmpty(objectiveText.text))
            objectivePanel.SetActive(false);
    }

    // Metodo helper per sapere a che punto siamo
    public string GetCurrentObjective()
    {
        return objectiveText.text;
    }

    // Metodo principale per cambiare obiettivo
    public void SetObjective(string newObjective)
    {
        // 1. Aggiorna il dato interno
        currentObjectiveID = newObjective;

        // 2. Aggiorna la UI
        objectivePanel.SetActive(true);

        // Avvia l'animazione solo se il testo cambia
        if (objectiveText.text != newObjective.ToUpper())
        {
            StartCoroutine(UpdateObjectiveRoutine(newObjective));
        }
    }

    // Coroutine per dare un feedback visivo (Lampeggio/Glitch semplice)
    private IEnumerator UpdateObjectiveRoutine(string text)
    {
        // 1. Fade Out (o cambio colore momentaneo)
        objectiveText.color = Color.yellow; // Feedback visivo: diventa giallo
        objectiveText.text = text.ToUpper(); // Forza maiuscolo come da copione

        // Opzionale: Qui potresti suonare un audio "NewObjective_SFX"
        // AudioManager.Instance.Play("ObjectiveUpdate");

        yield return new WaitForSeconds(fadeDuration);

        // 2. Ritorna al colore standard
        objectiveText.color = Color.black;
    }

    // Metodo per pulire l'obiettivo (es. fine gioco)
    public void ClearObjective()
    {
        currentObjectiveID = "";
        objectiveText.text = "";
        objectivePanel.SetActive(false);
    }
}