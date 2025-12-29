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
    public float displayDuration = 3f;  // Quanto tempo rimane il messaggio?

    private Coroutine hideCoroutine;
    private Coroutine currentRoutine;   // Per gestire (e fermare) la coroutine attiva

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Assicuriamoci che il pannello sia spento all'avvio
        if (dialogPanel != null) dialogPanel.SetActive(false);
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

    private IEnumerator TypeTextRoutine(string textToType)
    {
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
        yield return new WaitForSeconds(displayDuration);

        // --- FASE 3: CHIUSURA ---
        CloseDialog();
    }

    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
        dialogText.text = "";
    }
}