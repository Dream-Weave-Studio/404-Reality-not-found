using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject introPanel;       // Il pannello nero (Panel_IntroSequence)
    public CanvasGroup introCanvasGroup; // Il componente CanvasGroup sul pannello
    public TMP_Text letterText;         // Il testo (Text_HackerLetter)

    [Header("Content")]
    [TextArea(3, 10)]
    public string[] introLines;         // Qui copieremo le frasi del copione

    [Header("Settings")]
    public float typingSpeed = 0.02f;   // Velocità effetto macchina da scrivere
    public float fadeOutDuration = 2.0f;// Quanto ci mette il nero a sparire alla fine

    private int currentLineIndex = 0;
    private bool isTyping = false;      // Per evitare skip mentre scrive
    private bool introFinished = false;

    private void Start()
    {
        // Se il GameManager dice che l'intro è già finita, spegniti subito.
        if (GameManager.Instance.introFinished)
        {
            introPanel.SetActive(false);
            return; // Esci e non fare nulla
        }

        // Controllo stato originale
        if (GameManager.Instance.currentState != GameManager.GameState.IntroSequence)
        {
            introPanel.SetActive(false);
            return;
        }

        Time.timeScale = 1f;

        // Setup iniziale
        introPanel.SetActive(true);
        introCanvasGroup.alpha = 1; // Tutto nero

        // Debug per vedere in console quale velocità sta usando davvero
        Debug.Log("Intro partita. Typing Speed attuale: " + typingSpeed);

        StartCoroutine(TypeLine(introLines[currentLineIndex]));
    }

    private void Update()
    {
        // Gestiamo l'input solo se siamo nell'intro e non è finita
        if (introFinished) return;

        // Se il giocatore preme Spazio, Invio o Click sinistro
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                // TODO: Se volessimo completare subito la frase corrente (skip typing)
                // Implementazione futura
            }
            else
            {
                NextLine();
            }
        }
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < introLines.Length)
        {
            StartCoroutine(TypeLine(introLines[currentLineIndex]));
        }
        else
        {
            FinishIntro();
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        letterText.text = "";

        foreach (char c in line.ToCharArray())
        {
            letterText.text += c;
            // Qui potresti inserire un suono: AudioManager.Play("KeyboardClick");
            // Usa Realtime per ignorare eventuali Time.timeScale o lag del gioco
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;

        // Opzionale: Mostra un'icona "Premi per continuare" lampeggiante qui
    }

    private void FinishIntro()
    {
        introFinished = true;
        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        // 1. Aspetta un attimo prima di dissolvere
        yield return new WaitForSeconds(0.5f);

        // 2. Fade Out del pannello nero (usando CanvasGroup)
        float elapsedTime = 0;
        float startAlpha = introCanvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            introCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeOutDuration);
            yield return null;
        }

        introCanvasGroup.alpha = 0;
        introPanel.SetActive(false);

        // 3. COMUNICAZIONE FONDAMENTALE COL GAMEMANAGER
        // Diciamo al "Regista" che l'intro è finita.
        // Il GameManager sbloccherà i controlli e farà partire la sveglia.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndIntro();
        }
    }
}