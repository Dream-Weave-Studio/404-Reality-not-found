using UnityEngine;
using TMPro;
using UnityEngine.UI; // Necessario per gestire l'Image
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("Riferimenti UI In-Game")]
    public GameObject dialogPanel;      // Il contenitore Padre
    public Image portraitImage;         // L'immagine del volto (figlio)
    public TMP_Text dialogText;         // Il testo (figlio)

    [Header("Settings")]
    public float typingSpeed = 0.03f;   // Velocità scrittura (più basso = più veloce)
    public float displayDuration = 2f;  // Quanto tempo rimane il messaggio?
    public float readingSpeedPerWord = 0.4f; // Secondi per parola (regola a piacere)

    private Coroutine hideCoroutine;
    private Coroutine currentRoutine;   // Per gestire (e fermare) la coroutine attiva

    public bool IsDialogActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // PASSAGGIO DI CONSEGNE: Do al vecchio manager i riferimenti ai nuovi pannelli
            Instance.dialogPanel = this.dialogPanel;
            Instance.portraitImage = this.portraitImage;
            Instance.dialogText = this.dialogText;

            // Mi assicuro che il pannello sia spento all'inizio
            if (Instance.dialogPanel != null) Instance.dialogPanel.SetActive(false);

            Destroy(gameObject);
        }
    }

    // Metodo chiamato dagli oggetti
    public void ShowDialog(string text, Sprite portrait)
    {
        // 1. Setup Pannello e Ritratto
        dialogPanel.SetActive(true);
        dialogText.text = text;

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true); // Mostra foto se c'è
        }
        else
        {
            portraitImage.gameObject.SetActive(false); // Nascondi foto se manca
        }

        // 2. Ferma eventuali dialoghi precedenti per evitare sovrapposizioni
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        // 3. Avvia la scrittura
        currentRoutine = StartCoroutine(TypeTextRoutine(text));
    }

    private float CalculateReadTime(string text)
    {
        int wordCount = text.Split(' ').Length;
        float readTime = wordCount * readingSpeedPerWord;
        return Mathf.Max(readTime, displayDuration); // Mai meno del minimo
    }

    private IEnumerator TypeTextRoutine(string textToType)
    {
        IsDialogActive = true;
        dialogText.text = ""; // Pulisci il testo precedente

        // --- FASE 1: SCRITTURA ---
        foreach (char letter in textToType.ToCharArray())
        {
            dialogText.text += letter;
            // Aspetta un attimo prima della prossima lettera (ignorando il TimeScale per funzionare anche in pausa se serve)
            yield return new WaitForSeconds(typingSpeed);
        }

        // --- FASE 2: ATTESA ---
        // Il testo è completo. Aspettiamo X secondi in modo che il giocatore possa leggere.
        yield return new WaitForSeconds(CalculateReadTime(textToType));

        // --- FASE 3: CHIUSURA ---
        CloseDialog();
    }

    // Mostra il dialogo senza chiudersi automaticamente
    public void ShowDialogPersistent(string text, Sprite portrait)
    {
        // Ferma qualsiasi coroutine attiva (incluso il timer di chiusura)
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        dialogPanel.SetActive(true);
        dialogText.text = "";

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        // Avvia la scrittura MA senza il timer di chiusura finale
        currentRoutine = StartCoroutine(TypeTextPersistentRoutine(text));
    }

    private IEnumerator TypeTextPersistentRoutine(string textToType)
    {
        IsDialogActive = true;
        dialogText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        // Nessun WaitForSeconds + CloseDialog → rimane aperto
    }

    public void CloseDialog()
    {
        IsDialogActive = false;
        dialogPanel.SetActive(false);
        dialogText.text = "";
    }
}