using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera vcam2_5D;
    public CinemachineVirtualCamera vcamIso;

    public enum ViewMode { Side2_5D, Isometric }
    public ViewMode currentMode = ViewMode.Side2_5D;

    void Start()
    {
        Debug.Log("Prova");
        SwitchView(currentMode); // Imposta visuale iniziale
        Debug.Log("cambio telecamera baqwote");
    }

    void Update()
    {
        // Toggle visuale con BackQuote (`)
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ViewMode newMode = currentMode == ViewMode.Side2_5D ? ViewMode.Isometric : ViewMode.Side2_5D;
            SwitchView(newMode);
        }
    }

    public void SwitchView(ViewMode newMode)
    {
        currentMode = newMode;

        vcam2_5D.Priority = (newMode == ViewMode.Side2_5D) ? 10 : 0;
        vcamIso.Priority = (newMode == ViewMode.Isometric) ? 10 : 0;

        Debug.Log($"Visuale attuale: {currentMode}");
    }
}
