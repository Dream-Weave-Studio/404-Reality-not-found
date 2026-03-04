using UnityEngine;

public class SittingState : IPlayerState
{
    private readonly PlayerController player;
    private bool standUpTriggered = false;

   

    public SittingState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        standUpTriggered = false;
        Debug.Log("[State] Entrato nello stato SITTING");
    }

    public void Update()
    {
        // Blocca tutto finché l'animazione non è finita.
        // Una volta triggerato, non facciamo più nulla: 
        // ci penserà OnStandUpAnimationFinished a uscire dallo stato.
        if (standUpTriggered) return;

        if (player.HasMovementInput())
        {
            standUpTriggered = true;
            DialogManager.Instance?.CloseDialog();
            player.TriggerStandUp(); // Delega al PlayerController
        }
    }

    public void Exit()
    {
        GameObject bed = GameObject.FindWithTag("Bed"); // assegna il tag "Bed" al letto
        if (bed != null)
        {
            bed.gameObject.layer = LayerMask.NameToLayer("Default"); // Assicurati che il letto sia interagibile di nuovo
        }
        Debug.Log("[State] Uscito dallo stato SITTING");
    }
}