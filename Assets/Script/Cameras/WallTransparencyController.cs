using UnityEngine;

public class WallTransparencyController : MonoBehaviour
{
    public Material wallMaterial;
    public Transform playerTransform;
    public Camera mainCamera; // Assegna la tua Main Camera qui nell'Inspector
    public Vector3 treshold;

    [Range(0f, 5f)]
    public float offsetTowardsCamera = 1.0f; // Quanto spostare la sfera verso la camera

    void Update()
    {
        if (playerTransform != null && wallMaterial != null && mainCamera != null)
        {
            // WorldToViewportPoint restituisce:
            // X, Y = Posizione sullo schermo (0-1)
            // Z = Distanza dalla camera in metri! (Questo è quello che ci serve)
            Vector3 screenPos = mainCamera.WorldToViewportPoint(playerTransform.position + treshold);

            // Passiamo tutto il Vector3 (X, Y, Z)
            wallMaterial.SetVector("_PlayerPosition", screenPos);
        }
    }
}