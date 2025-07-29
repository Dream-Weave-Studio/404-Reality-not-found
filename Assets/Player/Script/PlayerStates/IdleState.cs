using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerMovement player;

    public IdleState(PlayerMovement player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // TODO: Esempio - animator.SetTrigger("Idle");
    }

    public void Update()
    {
        // Controllo input per transizione
        if (player.HasMovementInput())
        {
            player.TransitionToState(player.IsRunningInput() ? player.runningState : player.walkingState);
        }
    }

    public void Exit() { }
}
