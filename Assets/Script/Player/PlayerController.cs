using System.Collections;
using UnityEngine;

public class PlayerController : GameEntity
{
    [SerializeField] private MovementComponent movementComponent;

    #region Variabili e componenti

    private StateMachineController stateMachine;
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkingState walkingState;
    [HideInInspector] public RunningState runningState;

    private int blendHash;

    #endregion

    #region Unity Methods (Start, Update)

    protected override void Start()
    {
        base.Start();

        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance è null!");
            return;
        }

        if (movementComponent == null)
        {
            Debug.LogError("MovementComponent non è assegnato nell'Inspector!");
            return;
        }

        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);

        stateMachine = new StateMachineController();
        stateMachine.Initialize(idleState);

        blendHash = Animator.StringToHash("Blend");
    }

    void Update()
    {
        stateMachine.UpdateState();

        // Aggiorna il parametro float per Blend del BlendTree
        if (animator != null && movementComponent != null)
        {
            float normalized = 0f;
            if (movementComponent.MaxSpeed > 0f)
                normalized = Mathf.Clamp01(movementComponent.CurrentSpeed / movementComponent.MaxSpeed);

            animator.SetFloat(blendHash, normalized);  // ← Usa hash invece di stringa
        }
    }

    #endregion

    #region Gestione Stati del Giocatore

    public void TransitionToState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }
    #endregion



    public bool HasMovementInput() => movementComponent.HasMovementInput();
    public bool IsRunningInput() => movementComponent.IsRunningInput();
    public void HandleIsometricMovement() => movementComponent.HandleIsometricMovement();
}