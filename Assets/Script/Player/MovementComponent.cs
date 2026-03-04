using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("Velocità")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    [Header("Rotazione")]
    [Tooltip("Velocità di rotazione del personaggio verso la direzione di movimento")]
    public float rotationSpeed = 10f;

    // Proprietà pubbliche per il Blend Tree
    public float CurrentSpeed => HasMovementInput() ? (isRunning ? runSpeed : walkSpeed) : 0f;
    public float MaxSpeed => Mathf.Max(walkSpeed, runSpeed);

    private Vector2 inputDirection;
    private bool isRunning;

    #region Unity Methods (Start, Update, OnEnable)
    void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove += HandleMoveInput;
            InputManager.Instance.OnRun += HandleRunToggle;
        }
    }

    void OnDisable()
    {
        // Defensive: controlla se esiste prima di de-registrare
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove -= HandleMoveInput;
            InputManager.Instance.OnRun -= HandleRunToggle;
        }
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Utility di Stato Input

    public bool HasMovementInput() => inputDirection != Vector2.zero;
    public bool IsRunningInput() => isRunning;
    #endregion

    #region Gestione Input

    private void HandleMoveInput(Vector2 input) => inputDirection = input;
    private void HandleRunToggle(bool running) => isRunning = running;

    #endregion
}
