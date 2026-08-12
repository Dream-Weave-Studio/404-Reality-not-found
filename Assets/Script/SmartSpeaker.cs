using UnityEngine;
using System.Collections;
using static GameManager;

public class SmartSpeaker : InteractableObject
{
    [Header("Impostazioni Sveglia")]
    public int interactionCount = 0;

    [Tooltip("Se vuoi mantenere l'AudioSource locale (posizionale), assegnalo qui. Altrimenti lascia null e usa alarmClipId.")]
    public AudioSource alarmAudio;        // opzionale, legacy/local
    [Tooltip("AudioSource per il suono di stop (legacy).")]
    public AudioSource stopAudio;         // opzionale, legacy

    [Tooltip("Se usi AudioManager, inserisci qui l'id del clip per la sveglia (es. 'sveglia_loop').")]
    public string alarmClipId = "";

    [Tooltip("Se usi AudioManager, inserisci qui l'id del clip per il suono di stop (es. 'sveglia_stop').")]
    public string stopClipId = "";

    [Header("Asset Dialogo UI")]
    public Sprite ryoSleepyFace;
    public Sprite ryoAngryFace;
    public string speakerName = "Ryo";

    // riferimento all'AudioSource creato/gestito dall'AudioManager (se usato)
    private AudioSource alarmManagedSource;

    private void Awake()
    {
        // sicurezza: non far partire nulla in PlayOnAwake
        if (alarmAudio != null)
        {
            alarmAudio.playOnAwake = false;
            // assicurati che l'output sia il gruppo SFX (opzionale)
            if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
                alarmAudio.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        }
        if (stopAudio != null)
        {
            stopAudio.playOnAwake = false;
            if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
                stopAudio.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        }
    }

    private void Update()
    {
        bool active = GameManager.Instance.currentState == GameState.WakingUp
                   || GameManager.Instance.currentState == GameState.Gameplay;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = active;
    }

    public override bool IsInteractable()
    {
        return GameManager.Instance.currentState == GameState.WakingUp
            || GameManager.Instance.currentState == GameState.Gameplay;
    }

    public override void Interact()
    {
        // Sequenza sveglia: attiva durante WakingUp
        if (GameManager.Instance.currentState == GameState.WakingUp)
        {
            if (interactionCount >= 3) return;

            interactionCount++;

            if (interactionCount == 1)
                DialogManager.Instance.ShowDialog("Lexa stop!", ryoSleepyFace);
            else if (interactionCount == 2)
                DialogManager.Instance.ShowDialog("Lexa stoop!", ryoSleepyFace);
            else if (interactionCount == 3)
                StopAlarmSequence();

            return;
        }

        // Qualsiasi altro momento → comportamento standard di InteractableObject
        base.Interact();
    }

    // Chiamare questo per avviare la sveglia (es. da GameManager quando entra in WakingUp)
    public void StartAlarmSequence(float volume = 1f, float spatialBlend = 1f)
    {
        // Se esiste AudioSource locale, usalo (posizionale)
        if (alarmAudio != null)
        {
            alarmAudio.loop = true;
            alarmAudio.volume = volume;
            alarmAudio.spatialBlend = spatialBlend;
            alarmAudio.Play();
            return;
        }

        // Altrimenti prova con AudioManager usando alarmClipId
        if (!string.IsNullOrEmpty(alarmClipId) && AudioManager.Instance != null)
        {
            if (AudioManager.Instance.HasClip(alarmClipId))
            {
                // PlaySFXOnObject ritorna l'AudioSource usato sul GameObject target
                alarmManagedSource = AudioManager.Instance.PlaySFXOnObject(alarmClipId, gameObject, true, volume, spatialBlend);
            }
            else
            {
                Debug.LogWarning($"SmartSpeaker: alarmClipId '{alarmClipId}' non trovato in AudioManager");
            }
            return;
        }

        // Fallback: log se nessuna sorgente disponibile
        Debug.LogWarning("SmartSpeaker: nessun AudioSource locale o alarmClipId impostato per StartAlarmSequence");
    }

    private void StopAlarmSequence()
    {
        // Stop locale se presente
        if (alarmAudio != null)
        {
            if (alarmAudio.isPlaying) alarmAudio.Stop();
        }

        // Stop gestito dall'AudioManager (se abbiamo una source restituita)
        if (alarmManagedSource != null)
        {
            alarmManagedSource.Stop();
            alarmManagedSource = null;
        }

        // Riproduci suono di stop: preferisci stopAudio locale, altrimenti AudioManager
        if (stopAudio != null)
        {
            stopAudio.loop = false;
            stopAudio.Play();
        }
        else if (!string.IsNullOrEmpty(stopClipId) && AudioManager.Instance != null)
        {
            if (AudioManager.Instance.HasClip(stopClipId))
                AudioManager.Instance.PlayOneShot(stopClipId);
            else
                Debug.LogWarning($"SmartSpeaker: stopClipId '{stopClipId}' non trovato in AudioManager");
        }

        DialogManager.Instance.ShowDialog("Argh!! Lexa ho detto stooop!!", ryoAngryFace);

        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.HidePrompt();

        StartCoroutine(WaitDialogThenFade());
    }

    private IEnumerator WaitDialogThenFade()
    {
        yield return new WaitForSeconds(2.0f);

        IntroController.Instance.FinishIntro();

        yield return new WaitForSeconds(1.0f);

        DialogManager.Instance.ShowDialogPersistent(
            "Buongiorno Ryo, non dimenticarti di fare del buon movimento con W A S D o con l'Analogico sinistro del Controller",
            interactableData.interlocutorPortrait
        );
    }

    // Utility: ferma la sveglia se l'oggetto viene disattivato/distrutto
    private void OnDisable()
    {
        if (alarmAudio != null && alarmAudio.isPlaying) alarmAudio.Stop();
        if (alarmManagedSource != null) alarmManagedSource.Stop();
    }
}
