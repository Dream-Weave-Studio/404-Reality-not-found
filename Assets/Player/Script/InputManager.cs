using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerControls controls;

    public InputAction Zoom => controls.Camera.Zoom;
    public InputAction Interact => controls.Player.Interact;
    public InputAction Run => controls.Player.Run;
    public InputAction Move => controls.Player.Move;
    // Aggiungi altre azioni qui

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        controls = new PlayerControls();
        controls.Enable(); // Abilita tutte le mappe
        DontDestroyOnLoad(gameObject); // Se vuoi mantenerlo tra scene
    }

    void OnDestroy()
    {
        controls.Disable();
    }
}
