using UnityEngine;
using System.Collections;
using static GameManager;

public class SmartSpeaker : InteractableObject
{
    [Header("Impostazioni Sveglia")]
    public int interactionCount = 0;
    public AudioSource alarmAudio;
    public AudioSource stopAudio;

    [Header("Asset Dialogo UI")]
    public Sprite ryoSleepyFace;
    public Sprite ryoAngryFace;
    public string speakerName = "Ryo";

    private void Update()
    {
        bool active = GameManager.Instance.currentState == GameState.WakingUp
                   || GameManager.Instance.currentState == GameState.Gameplay;

        GetComponent<Collider>().enabled = active;
    }

    // Override: la sveglia bypassa la logica quest di IsInteractable()
    // ed è interagibile in base allo stato di gioco, non alle missioni
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

    private void StopAlarmSequence()
    {
        if (alarmAudio) alarmAudio.Stop();
        if (stopAudio) stopAudio.Play();

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
}