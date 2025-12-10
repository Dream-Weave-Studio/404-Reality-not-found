using UnityEngine;

public class StateMachineController
{
    private IPlayerState currentState;

    public void Initialize(IPlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
        Debug.Log($"[StateMachine] Stato iniziale: {startingState.GetType().Name}");
    }

    public void ChangeState(IPlayerState newState)
    {
        Debug.Log($"[StateMachine] Cambio stato da {currentState.GetType().Name} a {newState.GetType().Name}");
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateState()
    {
        currentState.Update();
    }

    public string GetCurrentStateName()
    {
        return currentState.GetType().Name;
    }
}
