using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gestisce tutti gli input del giocatore usando Unity Input System.
/// Rende disponibili eventi delegati per movimento, corsa, salto, interazione e zoom.
/// </summary>

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new PlayerControls();
    }
    #endregion

    #region Input System UI Support

    public enum InputType { Keyboard, Xbox, PlayStation, GenericGamepad }
    public InputType currentInputType = InputType.Keyboard;

    // Evento che la UI ascolterà per aggiornare le icone al volo
    public event Action<InputType> OnInputChanged;

    private void Update()
    {
        // Controllo semplice dell'ultimo dispositivo attivo
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            UpdateInputType(Gamepad.current);
        }
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame || Mouse.current.wasUpdatedThisFrame)
        {
            SetInputType(InputType.Keyboard);
        }
    }

    private void UpdateInputType(Gamepad gamepad)
    {
        // Rilevamento basico del tipo di controller
        // (Per una precisione assoluta servirebbero i Device Layouts, ma questo copre il 90% dei casi)
        string deviceName = gamepad.name.ToLower();

        if (deviceName.Contains("dualshock") || deviceName.Contains("dualsense") || deviceName.Contains("playstation"))
        {
            SetInputType(InputType.PlayStation);
        }
        else // Defaultiamo a Xbox per tutto il resto (XInput standard)
        {
            SetInputType(InputType.Xbox);
        }
    }

    private void SetInputType(InputType newType)
    {
        if (currentInputType != newType)
        {
            currentInputType = newType;
            Debug.Log($"Input cambiato in: {newType}");
            OnInputChanged?.Invoke(newType);
        }
    }

    #endregion


    #region Input System Setup
    private PlayerControls controls;

    // Espone direttamente l'InputAction dello zoom, utile per controlli analogici
    public InputAction Zoom => controls.Camera.Zoom;

    private void OnEnable()
    {
        controls.Enable();


        controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);


        controls.Player.Run.performed += ctx => OnRun?.Invoke(true);
        controls.Player.Run.canceled += ctx => OnRun?.Invoke(false);


        controls.Player.Interact.performed += ctx => OnInteract?.Invoke();

        controls.Camera.Zoom.performed += ctx => OnZoom?.Invoke(ctx.ReadValue<float>());
    }

    private void OnDisable()
    {
        // AGGIUNTA SICUREZZA: "controls" potrebbe essere null se l'oggetto 
        // viene distrutto nel Awake (Singleton pattern) prima di essere inizializzato.
        if (controls != null)
        {
            controls.Disable();
        }
    }

    #endregion

    #region Eventi di Input

    /// <summary>Evento quando il giocatore si muove (Vector2 da WASD o Stick Analogico)</summary>
    public event Action<Vector2> OnMove;

    /// <summary>Evento quando viene premuto il tasto di corsa (Shift o trigger)</summary>
    public event Action<bool> OnRun;

    /// <summary>Evento quando viene premuto il tasto salto</summary>
    public event Action OnJump;

    /// <summary>Evento per interazioni con oggetti/NPC</summary>
    public event Action OnInteract;

    /// <summary>Evento per zoom della camera (mouse scroll o analogico)</summary>
    public event Action<float> OnZoom;

    #endregion
}
