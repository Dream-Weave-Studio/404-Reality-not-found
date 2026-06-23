using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reazione che innesca un effetto glitch visivo sullo schermo per pochi secondi
/// bloccando temporaneamente il movimento del giocatore.
/// Il glitch avviene DOPO la prima battuta di Ryo, seguito da una seconda battuta
/// e dallo sblocco del nuovo obiettivo "pillole". Avviene solo una volta.
/// </summary>
public class GlitchReaction : InteractionReaction
{
    [Header("Parametri Glitch")]
    [Tooltip("Durata dell'effetto glitch in secondi.")]
    public float duration = 2.0f;

    [Tooltip("Intensità del movimento orizzontale delle barre di glitch.")]
    public float displacementIntensity = 100f;

    [Tooltip("Frequenza di aggiornamento del glitch (tempo tra aggiornamenti dei frame).")]
    public float glitchFlickerInterval = 0.08f;

    [Tooltip("Tempo aggiuntivo di attesa dopo la scrittura del primo messaggio prima di forzare il glitch.")]
    public float firstMessageReadBuffer = 0.8f;

    private bool hasTriggered = false;

    protected override void PerformReaction(GameObject interactor)
    {
    
        if (hasTriggered) return;

        if (MemoryManager.Instance != null && MemoryManager.Instance.CheckFact("frigo_glitch_triggered"))
        {
            hasTriggered = true;
            return;
        }

        hasTriggered = true;

        if (MemoryManager.Instance != null)
        {
            MemoryManager.Instance.SetFact("frigo_glitch_triggered");
        }

        StartCoroutine(GlitchRoutine());
    }

    private IEnumerator GlitchRoutine()
    {
        InteractableObject interactable = GetComponent<InteractableObject>();
        Sprite ryoPortrait = interactable != null && interactable.interactableData != null 
            ? interactable.interactableData.characterPortrait 
            : null;

        yield return null;

        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
        {
            string firstText = DialogManager.Instance.CurrentFullText;
            float typingDuration = firstText.Length * DialogManager.Instance.typingSpeed;
            float waitTimeBeforeGlitch = typingDuration + firstMessageReadBuffer;

            float timer = 0f;
            while (timer < waitTimeBeforeGlitch && DialogManager.Instance.IsDialogActive)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (DialogManager.Instance.IsDialogActive)
            {
                DialogManager.Instance.CloseDialog();
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Cutscene);
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        PlayerController player = playerObj != null ? playerObj.GetComponent<PlayerController>() : null;
        if (player != null)
        {
            player.TransitionToState(player.idleState);
        }

        GameObject canvasObj = new GameObject("GlitchCanvasOverlay");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99999;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject bgObj = new GameObject("GlitchBG");
        bgObj.transform.SetParent(canvasObj.transform, false);
        var bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0.2f, 0f, 0f, 0.3f);
        var bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        CameraManager camManager = FindObjectOfType<CameraManager>();
        float originalIsoZoom = 1f;
        if (camManager != null && camManager.vcamIso != null)
        {
            originalIsoZoom = camManager.vcamIso.m_Lens.OrthographicSize;
        }

        float elapsed = 0f;
        List<GameObject> activeGlitchBars = new List<GameObject>();

        while (elapsed < duration)
        {

            bgImage.color = new Color(
                Random.Range(0.15f, 0.35f),
                0f,
                0f,
                Random.Range(0.2f, 0.5f)
            );

            foreach (var bar in activeGlitchBars)
            {
                if (bar != null) Destroy(bar);
            }
            activeGlitchBars.Clear();

            int barCount = Random.Range(4, 12);
            for (int i = 0; i < barCount; i++)
            {
                GameObject barObj = new GameObject("GlitchBar_" + i);
                barObj.transform.SetParent(canvasObj.transform, false);
                var barImage = barObj.AddComponent<UnityEngine.UI.Image>();

                Color[] glitchColors = new Color[] {
                    new Color(1f, 0f, 0f, Random.Range(0.6f, 1f)),        // Rosso puro
                    new Color(0.6f, 0f, 0f, Random.Range(0.6f, 1f)),      // Rosso scuro
                    new Color(1f, 0.25f, 0.25f, Random.Range(0.6f, 1f)),  // Rosso chiaro
                    new Color(0.2f, 0f, 0f, Random.Range(0.7f, 0.95f)),   // Quasi nero/rosso cupo
                    new Color(1f, 1f, 1f, Random.Range(0.3f, 0.6f))       // Bianco ad alta luminosità
                };
                barImage.color = glitchColors[Random.Range(0, glitchColors.Length)];

                var barRect = barObj.GetComponent<RectTransform>();
                float yMin = Random.Range(0f, 1f);
                float yMax = yMin + Random.Range(0.005f, 0.08f);

                barRect.anchorMin = new Vector2(0f, yMin);
                barRect.anchorMax = new Vector2(1f, yMax);
                barRect.sizeDelta = Vector2.zero;

                float xOffset = Random.Range(-displacementIntensity, displacementIntensity);
                barRect.anchoredPosition = new Vector2(xOffset, 0f);

                activeGlitchBars.Add(barObj);
            }

            if (camManager != null && camManager.vcamIso != null)
            {
                camManager.vcamIso.m_Lens.OrthographicSize = originalIsoZoom + Random.Range(-1.2f, 1.2f);
            }

            yield return new WaitForSecondsRealtime(glitchFlickerInterval);
            elapsed += glitchFlickerInterval;
        }

        foreach (var bar in activeGlitchBars)
        {
            if (bar != null) Destroy(bar);
        }
        Destroy(canvasObj);

        if (camManager != null && camManager.vcamIso != null)
        {
            camManager.vcamIso.m_Lens.OrthographicSize = originalIsoZoom;
        }

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.ShowDialog("dove sono quelle maledettissime pillole", ryoPortrait);

            while (DialogManager.Instance.IsDialogActive)
            {
                yield return null;
            }
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UnlockSubsByEvent("opened_frigo");
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Gameplay);
        }
    }
}
