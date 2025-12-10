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
        // 1. Imposta il contenuto
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

        // 2. Attiva il pannello
        dialogPanel.SetActive(true);

        // 3. Gestione Timer: Se c'era già un timer per nascondere, lo fermiamo
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Avviamo un nuovo timer per nascondere il pannello
        hideCoroutine = StartCoroutine(HideDialogAfterDelay());
    }

    private IEnumerator HideDialogAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        // Disattiva il pannello
        dialogPanel.SetActive(false);
        hideCoroutine = null;
    }
}