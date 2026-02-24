using UnityEngine;

/// <summary>
/// Reazione che modifica la scala di un oggetto.
/// Con pivot verso l'alto si ottiene l'animazione della persiana
/// </summary>
public class ScaleReaction : InteractionReaction
{
    [Header("Configurazione Scala")]
    [Tooltip("L'oggetto da scalare. Se vuoto, scala se stesso.")]
    public Transform targetObject;

    [Tooltip("La scala target da raggiungere (es. 1,1,1 per dimensione normale).")]
    public Vector3 targetScale = new Vector3(1, 1, 1);

    [Tooltip("Se vero, parte da scala zero (o molto piccola) all'avvio del gioco.")]
    public bool startSmall = false;

    [Tooltip("Durata dell'animazione in secondi.")]
    public float duration = 1.0f;

    [Tooltip("Curva di animazione (es. EaseInOut o Linear).")]
    public AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Logica")]
    [Tooltip("Se vero, al click successivo inverte l'animazione.")]
    public bool toggle = true;

    // Stato interno
    private bool isScaled = false;
    private Vector3 initialScale;

    private void Awake()
    {
        if (targetObject == null)
            targetObject = transform;

        initialScale = targetObject.localScale;

        if (startSmall)
        {
            targetScale = initialScale; // Quella che vedi in scena è l'obiettivo
            initialScale = new Vector3(initialScale.x, 0.01f, initialScale.z); // Partiamo da "chiuso"
            targetObject.localScale = initialScale;
            isScaled = false; // Siamo nello stato "non scalato" (chiuso)
        }
    }

    protected override void PerformReaction(GameObject interactor)
    {
        Vector3 destinationScale;

        // Logica Toggle
        if (toggle)
        {
            if (isScaled)
            {
                // Torniamo alla scala iniziale
                destinationScale = initialScale;
            }
            else
            {
                // Andiamo alla scala target 
                destinationScale = targetScale;
            }
            isScaled = !isScaled;
        }
        else
        {
            // Oneshot: andiamo al target e basta
            if (isScaled) return;
            destinationScale = targetScale;
            isScaled = true;
        }

        StopAllCoroutines();
        StartCoroutine(ScaleRoutine(destinationScale));
    }

    private System.Collections.IEnumerator ScaleRoutine(Vector3 endScale)
    {
        Vector3 startScale = targetObject.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curveValue = animationCurve.Evaluate(t);

            targetObject.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetObject.localScale = endScale;
    }
}