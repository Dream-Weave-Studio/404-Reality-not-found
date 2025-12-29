using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

/// <summary>
/// Gestisce la visuale della camera (2.5D vs Isometrica),
/// il suo zoom reattivo, e la transizione tra modalità.
/// </summary>
public class CameraManager : MonoBehaviour
{
    #region Riferimenti e impostazioni

    public CinemachineVirtualCamera vcam2_5D;
    public CinemachineVirtualCamera vcamIso;

    public enum ViewMode { Side2_5D, Isometric }
    public ViewMode currentMode = ViewMode.Side2_5D;

    
    public float zoomSensitivity_mouse;
    public float zoomSensitivity_stick;

    public float minZoom;
    public float maxZoom;

    public float zoomLagSpeed = 10f; // Più basso = risposta più lenta


    [Header("Debug")]
    [SerializeField] float targetZoom;

    #endregion

    #region Inizializzazione

    void Start()
    {
        //InitInput();
        targetZoom = vcamIso.m_Lens.OrthographicSize;

        SwitchView(currentMode); // Imposta visuale iniziale

        vcam2_5D.Follow = FindPlayerTransform(); // opzionale per 2.5D
        vcamIso.Follow = FindPlayerTransform();

    }
    #endregion

    #region Gestione visuale e zoom
    void Update()
    {
        HandleViewSwitchInput();
        HandleZoomInput();
        ApplyZoomSmoothing();
    }
    void HandleViewSwitchInput()
    {
        // AGGIUNTA: Se non siamo in Gameplay, ignora il tasto di cambio visuale
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Gameplay)
            return;


        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            ViewMode newMode = currentMode == ViewMode.Side2_5D ? ViewMode.Isometric : ViewMode.Side2_5D;
            SwitchView(newMode);
        }
    }
    void HandleZoomInput()
    {
        // AGGIUNTA: Se non siamo in Gameplay, ignora lo scroll/stick
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Gameplay)
            return;

        float scrollInput = InputManager.Instance.Zoom.ReadValue<float>();
        var device = InputManager.Instance.Zoom.activeControl?.device;

        if (scrollInput != 0)
        {
            float sensitivity = zoomSensitivity_mouse;
            if (device is Gamepad)
                sensitivity *= zoomSensitivity_stick;

            targetZoom -= scrollInput * sensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    void ApplyZoomSmoothing()
    {
        vcamIso.m_Lens.OrthographicSize = Mathf.MoveTowards(
            vcamIso.m_Lens.OrthographicSize,
            targetZoom,
            zoomLagSpeed * Time.deltaTime
        );
    }
    #endregion

    #region Utility
    Transform FindPlayerTransform()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }


    public void SwitchView(ViewMode newMode)
    {
        currentMode = newMode;

        vcam2_5D.Priority = (newMode == ViewMode.Side2_5D) ? 10 : 0;
        vcamIso.Priority = (newMode == ViewMode.Isometric) ? 10 : 0;

        Debug.Log($"Visuale attuale: {currentMode}");
    }
    #endregion
}
