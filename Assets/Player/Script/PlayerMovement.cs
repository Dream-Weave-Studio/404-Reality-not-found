using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Per usare il nuovo Input System

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private Vector2 moveInput;
    private bool isRunning;

    private StateMachineController stateMachine;

    [HideInInspector] public IdleState idleState;
    [HideInInspector] public WalkingState walkingState;
    [HideInInspector] public RunningState runningState;



    void OnEnable()
    {
        InputManager.Instance.Run.performed += OnRunStarted;
        InputManager.Instance.Run.canceled += OnRunCanceled;
        InputManager.Instance.Interact.performed += HandleInteraction;
    }

    void OnDisable()
    {
        InputManager.Instance.Run.performed -= OnRunStarted;
        InputManager.Instance.Run.canceled -= OnRunCanceled;
        InputManager.Instance.Interact.performed -= HandleInteraction;
    }

    private void OnRunStarted(InputAction.CallbackContext ctx) => isRunning = true;
    private void OnRunCanceled(InputAction.CallbackContext ctx) => isRunning = false;
    private void HandleInteraction(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interazione avviata!");
    }

    void Start()
    {
        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);

        stateMachine = new StateMachineController();
        stateMachine.Initialize(idleState);


        InputManager.Instance.Run.performed += ctx => isRunning = true;
        InputManager.Instance.Interact.performed += ctx => HandleInteraction();
    }

    void Update()
    {
        moveInput = InputManager.Instance.Move.ReadValue<Vector2>();
        HandleIsometricMovement();
        stateMachine.UpdateState();
    }

    public void TransitionToState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public bool HasMovementInput()
    {
        return moveInput != Vector2.zero;
    }

    public bool IsRunningInput()
    {
        return isRunning;
    }

    public void HandleIsometricMovement()
    {
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Quaternion isoRotation = Quaternion.Euler(0, 45f, 0);
        Vector3 rotatedInput = isoRotation * input;

        transform.Translate(rotatedInput * speed * Time.deltaTime, Space.World);

        if (rotatedInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotatedInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    private void HandleInteraction()
    {
        Debug.Log("Interazione avviata!");
    }

}
