using System.Collections;
using UnityEngine;

/// <summary>
/// Gestisce il movimento base del giocatore: camminare e correre.
/// </summary>


public class PlayerMovement : MonoBehaviour
{
    #region Variabili e componenti
    [Header("Velocità")] // Settings visibili nell'Inspector
    public float walkSpeed = 3f;
    public float runSpeed = 6f;


    // Componenti e variabili interne
    private Rigidbody rb;
    private Vector2 inputDirection;
    private bool isRunning;

    // StateMachine
    private StateMachineController stateMachine;
    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkingState walkingState;
    [HideInInspector] public RunningState runningState;
    #endregion

    #region Unity Methods (Start, Update, OnEnable)
    void OnEnable()
    {
        InputManager.Instance.OnMove += HandleMoveInput;
        InputManager.Instance.OnRun += HandleRunToggle;
        InputManager.Instance.OnInteract += HandleInteraction;
    }

    void OnDisable()
    {
        InputManager.Instance.OnMove -= HandleMoveInput;
        InputManager.Instance.OnRun -= HandleRunToggle;
        InputManager.Instance.OnInteract -= HandleInteraction;
    }

    void Start()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance è null!");
            return;
        }

        // ?? Inizializza gli stati
        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);

        stateMachine = new StateMachineController();
        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        HandleIsometricMovement();
        stateMachine.UpdateState();
    }

    #endregion

    #region Gestione Stati del Giocatore

    /// <summary>
    /// Cambia lo stato attuale del giocatore nella State Machine.
    /// Viene chiamato da uno stato (Idle, Walking, Running) per passare a un altro.
    /// </summary>
    public void TransitionToState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }
    #endregion

    #region Utility di Stato Input

    /// <summary>
    /// True se il giocatore sta fornendo input di movimento.
    /// </summary>
    public bool HasMovementInput()
    {
        return inputDirection != Vector2.zero;
    }

    public bool IsRunningInput()
    {
        return isRunning;
    }
    #endregion

    #region Movimento Isometrico

    /// <summary>
    /// Applica movimento isometrico basato sull’input.
    /// </summary>
    public void HandleIsometricMovement()
    {
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 input = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;
        Quaternion isoRotation = Quaternion.Euler(0, 45f, 0);
        Vector3 rotatedInput = isoRotation * input;

        transform.Translate(rotatedInput * speed * Time.deltaTime, Space.World);

        if (rotatedInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotatedInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    #endregion

    #region Gestione Input

    /// <summary>
    /// Riceve input di movimento dal sistema di input.
    /// </summary>
    private void HandleMoveInput(Vector2 input)
    {
        inputDirection = input;
    }

    private void HandleRunToggle(bool isRunning)
    {
        this.isRunning = isRunning;
    }

    private void HandleInteraction()
    {
        Debug.Log("Interazione avviata!");
    }
    #endregion
}
