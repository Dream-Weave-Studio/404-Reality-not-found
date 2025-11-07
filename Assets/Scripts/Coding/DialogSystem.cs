using UnityEngine;
using UnityEngine.UI;   // Per i componenti UI standard
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance { get; private set; }
    public bool IsLetterClosed { get; private set; }

    // Lista dei messaggi ricevuti (log)
    private List<string> hackerMessages = new List<string>();

    [Header("UI Hacker Letter")]
    [SerializeField] private GameObject hackerLetterPanel;
    [SerializeField] private Text hackerLetterText;

    [Header("UI Dialogue Box (pensieri di Ryo)")]
    [SerializeField] private GameObject dialogueBoxPanel;
    [SerializeField] private Text dialogueBoxText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Metodo originale: mostra e salva un messaggio (console + log)
    public IEnumerator ShowLetter(string text)
    {
        IsLetterClosed = false;

        hackerMessages.Add(text);

        Debug.Log("Messaggio Hacker: " + text);
        Debug.Log("Premi SPAZIO per chiudere...");

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        IsLetterClosed = true;
        Debug.Log("Lettera chiusa!");
    }

    // Recupera tutti i messaggi salvati
    public List<string> GetAllMessages()
    {
        return hackerMessages;
    }

    // ?? NUOVO: Mostra la lettera dell’Hacker su UI
    public IEnumerator ShowHackerLetter(string text)
    {
        IsLetterClosed = false;

        // Salva nel log
        hackerMessages.Add(text);

        // Attiva pannello e inserisce testo
        if (hackerLetterPanel != null && hackerLetterText != null)
        {
            hackerLetterPanel.SetActive(true);
            hackerLetterText.text = text;
        }

        // Attesa input giocatore (es. SPAZIO per chiudere)
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        // Nascondi pannello
        if (hackerLetterPanel != null)
            hackerLetterPanel.SetActive(false);

        IsLetterClosed = true;
    }

    // ?? NUOVO: Mostra pensieri/dialoghi di Ryo
    public void ShowDialogueLine(string text)
    {
        if (dialogueBoxPanel != null && dialogueBoxText != null)
        {
            dialogueBoxPanel.SetActive(true);
            dialogueBoxText.text = text;
        }
    }

    // Metodo per nascondere il box dialogo (quando serve)
    public void HideDialogueBox()
    {
        if (dialogueBoxPanel != null)
            dialogueBoxPanel.SetActive(false);
    }
}
