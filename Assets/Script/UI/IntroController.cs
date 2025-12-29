using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public static IntroController Instance; // 1. Diventa Singleton

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

    private void Awake()
    {
        if (Instance == null)
        {
            // 2. Setup iniziale del Sopravvissuto
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 3. REFRESH: Il nuovo IntroController (della scena ricaricata) 
            // passa i collegamenti freschi al vecchio IntroController (Sopravvissuto)

            Instance.introPanel = this.introPanel;
            Instance.introCanvasGroup = this.introCanvasGroup;
            Instance.letterText = this.letterText;

            // Passiamo anche i dati del contenuto, nel caso avessi cambiato testo nell'inspector
            Instance.introLines = this.introLines;

            // Se l'intro era già finita nella sessione precedente, assicura che il pannello sia spento
            if (GameManager.Instance != null && GameManager.Instance.introFinished)
            {
                if (Instance.introPanel != null) Instance.introPanel.SetActive(false);
            }

            // 4. Il nuovo si sacrifica
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Se non sono l'istanza ufficiale, non faccio nulla (sto per essere distrutto)
        if (Instance != this) return;

        CheckIntroStart();
    }

    // Spostiamo la logica di avvio in un metodo pubblico/riutilizzabile
    public void CheckIntroStart()
    {
        // Se il GameManager dice che l'intro è già finita...
        if (GameManager.Instance != null && GameManager.Instance.introFinished)
        {
            if (introPanel != null) introPanel.SetActive(false);
            return;
        }

        // Se lo stato non è IntroSequence...
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.IntroSequence)
        {
            if (introPanel != null) introPanel.SetActive(false);
            return;
        }

        // --- AVVIO INTRO ---
        Time.timeScale = 1f;
        currentLineIndex = 0; // Reset indice

        if (introPanel != null)
        {
            introPanel.SetActive(true);
            if (introCanvasGroup != null) introCanvasGroup.alpha = 1;
        }

        if (introLines != null && introLines.Length > 0)
        {
            StartCoroutine(TypeLine(introLines[currentLineIndex]));
        }
    }

    private void Update()
    {
        if (Instance != this) return; // Solo il sopravvissuto lavora
        if (introFinished) return;

        // Input per avanzare
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
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
        if (letterText != null) letterText.text = "";

        foreach (char c in line.ToCharArray())
        {
            if (letterText != null) letterText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    private void FinishIntro()
    {
        introFinished = true;
        StartCoroutine(FadeOutSequence());
    }

    private IEnumerator FadeOutSequence()
    {
        yield return new WaitForSeconds(0.5f);

        float elapsedTime = 0;
        float startAlpha = (introCanvasGroup != null) ? introCanvasGroup.alpha : 1;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            if (introCanvasGroup != null)
                introCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeOutDuration);
            yield return null;
        }

        if (introCanvasGroup != null) introCanvasGroup.alpha = 0;
        if (introPanel != null) introPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndIntro();
        }
    }
}