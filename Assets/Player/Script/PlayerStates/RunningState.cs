using UnityEngine;

public class RunningState : IPlayerState
{
    private PlayerMovement player;

    public RunningState(PlayerMovement playerMovement)
    {
        this.player = playerMovement;
    }

    public void Enter()
    {
        Debug.Log("[State] Entrato nello stato RUNNING");
    }

    public void Exit()
    {
        Debug.Log("[State] Uscito dallo stato RUNNING");
    }

    public void Update()
    {
        // Nessun input? Torna a idle
        if (!player.HasMovementInput())
        {
            player.TransitionToState(player.idleState);
            return;
        }

        // Se il tasto Shift non è premuto, torna a camminare
        if (!player.IsRunningInput())
        {
            player.TransitionToState(player.walkingState);
            return;
        }

        // Movimento tramite metodo isometrico
        player.HandleIsometricMovement();
    }
}
