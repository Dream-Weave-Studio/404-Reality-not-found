using UnityEngine;
using System.Collections;

/// <summary>
/// Gestisce la sequenza PC: Nera -> Spam -> Mail.
/// Eredita da InteractionReaction per essere "pluggato" direttamente negli InteractableObject.
/// </summary>
public class ScreenSequenceReaction : InteractionReaction
{
    [Header("Rendering")]
    [Tooltip("Il Renderer dello schermo (o del PC intero). Se null, usa GetComponent<Renderer>().")]
    public Renderer screenRenderer;

    [Tooltip("L'indice del materiale da cambiare (0 se mesh unica).")]
    public int screenMaterialIndex = 0;

    [Header("Sequenza Materiali (Update -> Black -> Mail)")]
    [Tooltip("Fase 1: Update (Stato Iniziale/Idle).")]
    public Material updateMaterial;
    [Tooltip("Fase 2: Black (Schermata Nera passaggio tempo).")]
    public Material blackMaterial;
    [Tooltip("Fase 3: Mail (Desktop Finale).")]
    public Material mailMaterial;

    [Header("Configurazione Tempi")]
    [Tooltip("Durata della schermata nera.")]
    public float blackScreenDuration = 4.5f;


    // Stato interno
    private bool isBooted = false;
    private bool isSequenceStarted = false;

    private void Awake()
    {
        if (screenRenderer == null) screenRenderer = GetComponent<Renderer>();

        // Setup iniziale: BLACK (Spento)
        if (blackMaterial != null) ApplyMaterial(blackMaterial);
    }

    protected override void PerformReaction(GameObject interactor)
    {
        // STEP 1: Accensione
        if (!isBooted)
        {
            isBooted = true;
            if (updateMaterial != null) ApplyMaterial(updateMaterial);
            return;
        }


        // STEP 2: Conferma -> Schermo Nero -> Mail
        if (!isSequenceStarted)
        {
            isSequenceStarted = true;
            StartCoroutine(SequenceRoutine());
        }
    }

    private IEnumerator SequenceRoutine()
    {
        // Subito nero dopo il secondo click
        ApplyMaterial(blackMaterial);
        yield return new WaitForSeconds(blackScreenDuration);

        // MAIL (Finale)
        ApplyMaterial(mailMaterial);

    }

    private void ApplyMaterial(Material mat)
    {
        if (mat == null || screenRenderer == null) return;

        Material[] mats = screenRenderer.materials;
        if (screenMaterialIndex >= 0 && screenMaterialIndex < mats.Length)
        {
            mats[screenMaterialIndex] = mat;
            screenRenderer.materials = mats;
        }
    }
}
