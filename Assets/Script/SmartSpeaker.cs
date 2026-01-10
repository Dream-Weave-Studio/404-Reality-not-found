using UnityEngine;
using static GameManager;

public class SmartSpeaker : InteractableObject
{
    [Header("Impostazioni Sveglia")]
    public int interactionCount = 0; // Conta i tentativi
    public AudioSource alarmAudio;   // Il 'bip bip'
    public AudioSource stopAudio;    // Il suono di conferma

    [Header("Asset Dialogo UI")]
    // Ci servono le facce di Ryo per la UI
    public Sprite ryoSleepyFace; // Per i primi tentativi
    public Sprite ryoAngryFace;  // Per l'urlo finale
    public string speakerName = "Ryo"; // Chi sta parlando

    private void Update()
    {
        // Se siamo in Intro, Lexa non deve essere "interagibile"
        if (GameManager.Instance.currentState == GameState.IntroSequence)
        {
            GetComponent<Collider>().enabled = false; // Disabilita fisicamente il trigger
        }
        else
        {
            GetComponent<Collider>().enabled = true; // Riabilita quando inizia il Gameplay
        }
    }

    // OVERRIDE: Sostituiamo la logica standard con quella della sveglia
    public override void Interact()
    {
        // 1. CONTROLLO OBIETTIVO
        // Se NON è mattina (obiettivo diverso da "SVEGLIA_MATTINA"), comportati come un oggetto normale.
        // 'base.Interact()' chiama il codice dello script genitore (dialoghi, item, ecc.)
        if (ObjectiveManager.Instance == null || ObjectiveManager.Instance.GetCurrentObjective() != "SVEGLIA_MATTINA")
        {
            base.Interact();
            return;
        }

        // 2. LOGICA SPECIALE (Siamo nella fase sveglia)
        interactionCount++;

        if (interactionCount == 1)
        {
            // Primo tentativo
            Debug.Log("Ryo: Lexa stop!");
            DialogManager.Instance.ShowDialog("Lexa stop!", ryoSleepyFace);
        }
        else if (interactionCount == 2)
        {
            // Secondo tentativo
            Debug.Log("Ryo: Lexa stoop!");
            DialogManager.Instance.ShowDialog("Lexa stoop!", ryoSleepyFace);
        }
        else if (interactionCount >= 3)
        {
            // TERZO TENTATIVO: SUCCESSO
            StopAlarmSequence();
        }
    }

    private void StopAlarmSequence()
    {
        // Ferma il suono fastidioso e suona conferma
        if (alarmAudio) alarmAudio.Stop();
        if (stopAudio) stopAudio.Play();

        Debug.Log("Ryo: argh!! Lexa ho detto stooop!!");
        DialogManager.Instance.ShowDialog("Argh!! Lexa ho detto stooop!!", ryoAngryFace);

        // CAMBIO STATO: Aggiorna l'obiettivo nel Manager
        // Questo farà scattare la UI: "OBIETTIVO: TROVA IL TELEFONO"
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("TROVA IL TELEFONO");
        }

        // OPZIONALE: Resettiamo il contatore o disabilitiamo l'interazione speciale
        // Da ora in poi, l'if iniziale fallirà e l'oggetto diventerà "normale"
    }
}