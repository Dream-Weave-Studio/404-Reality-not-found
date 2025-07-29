using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{

    public CinemachineVirtualCamera vcam2_5D;
    public CinemachineVirtualCamera vcamIso;

    public enum ViewMode { Side2_5D, Isometric }
    public ViewMode currentMode = ViewMode.Side2_5D;

    private float targetZoom;
    public float zoomSensitivity_mouse;
    public float zoomSensitivity_stick;
    float zoomSensitivity;

    public float zoomLagSpeed = 10f; // Più basso = risposta più lenta

    void Start()
    {
        //InitInput();
        targetZoom = vcamIso.m_Lens.OrthographicSize;

        SwitchView(currentMode); // Imposta visuale iniziale

        vcam2_5D.Follow = FindPlayerTransform(); // opzionale per 2.5D
        vcamIso.Follow = FindPlayerTransform();

    }

    void Update()
    {

        if (Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            ViewMode newMode = currentMode == ViewMode.Side2_5D ? ViewMode.Isometric : ViewMode.Side2_5D;
            SwitchView(newMode);
        }
        float scrollInput = InputManager.Instance.Zoom.ReadValue<float>();
        var device = InputManager.Instance.Zoom.activeControl?.device;

        Debug.Log($"Zoom input from device: {device?.GetType().Name}");

        if (scrollInput != 0)
        {
            float sensitivity = zoomSensitivity_mouse;

            if (device is UnityEngine.InputSystem.Gamepad)
            {
                sensitivity *= zoomSensitivity_stick; 
            }


            targetZoom -= scrollInput * sensitivity;
            targetZoom = Mathf.Clamp(targetZoom, 4f, 12f);
        }

        vcamIso.m_Lens.OrthographicSize = Mathf.MoveTowards(
            vcamIso.m_Lens.OrthographicSize,
            targetZoom,
            zoomLagSpeed * Time.deltaTime
        );

    }
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
}
