using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("Velocità")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private Vector2 inputDirection;
    private bool isRunning;

    #region Unity Methods (Start, Update, OnEnable)
    void OnEnable()
    {
        InputManager.Instance.OnMove += HandleMoveInput;
        InputManager.Instance.OnRun += HandleRunToggle;
    }

    void OnDisable()
    {
        InputManager.Instance.OnMove -= HandleMoveInput;
        InputManager.Instance.OnRun -= HandleRunToggle;
    }

    
    #endregion

    #region Movimento Isometrico

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

    #region Utility di Stato Input

    public bool HasMovementInput()
    {
        return inputDirection != Vector2.zero;
    }

    public bool IsRunningInput()
    {
        return isRunning;
    }
    #endregion

    #region Gestione Input

    private void HandleMoveInput(Vector2 input)
    {
        // Se il GameManager esiste e NON siamo in fase di Gameplay, annulliamo il movimento.
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Gameplay)
        {
            inputDirection = Vector2.zero; // Forza lo stop
            return;
        }

        inputDirection = input;
    }

    private void HandleRunToggle(bool isRunning)
    {
        // AGGIUNTA: Stesso controllo per la corsa
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Gameplay)
        {
            this.isRunning = false; // Forza la camminata
            return;
        }

        this.isRunning = isRunning;
    }

    #endregion
}