using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : IPlayerState
{
    private PlayerController player;

    public WalkingState(PlayerController playerMovement)
    {
        this.player = playerMovement;
    }

    public void Enter()
    {
        Debug.Log("[State] Entrato nello stato WALKING");
    }

    public void Exit()
    {
        Debug.Log("[State] Uscito dallo stato WALKING");
    }

    public void Update()
    {
        // Verifica se sta ancora camminando
        if (!player.HasMovementInput())
        {
            player.TransitionToState(player.idleState);
            return;
        }

        if (player.IsRunningInput())
        {
            player.TransitionToState(player.runningState);
            return;
        }

        // Movimento tramite metodo isometrico
        player.HandleIsometricMovement();
    }
}
