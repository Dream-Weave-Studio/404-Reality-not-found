using UnityEngine;

public class CameraObstructionFader : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public LayerMask wallLayer;

    [Header("Animation Settings")]
    public float fadeSpeed = 10f;
    public float closeSpeed = 5f;
    public float rayRadius = 0.2f;
    public float heightOffset = 1.5f;

    [Header("Visual Style (Live Edit)")]
    public float holeRadius = 1.5f;   // Controlla la grandezza del buco
    public float pixelDensity = 30f;  // Controlla la grossezza dei pixel

    // Debug Visivo
    private Vector3 debugRayStart;
    private Vector3 debugRayEnd;
    private bool isHitDebug;

    void Update()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + Vector3.up * heightOffset;
        Vector3 dir = targetPos - transform.position;
        float dist = dir.magnitude;

        debugRayStart = transform.position;
        debugRayEnd = targetPos;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, rayRadius, dir, dist, wallLayer);
        isHitDebug = hits.Length > 0;

        foreach (RaycastHit hit in hits)
        {
            WallFader fader = hit.collider.GetComponent<WallFader>();
            if (fader == null) fader = hit.collider.GetComponentInParent<WallFader>();
            if (fader == null) fader = hit.collider.gameObject.AddComponent<WallFader>();

            // Passiamo TUTTI i parametri visivi ogni frame
            fader.StayOpen(targetPos, fadeSpeed, closeSpeed, holeRadius, pixelDensity);
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;
        Gizmos.color = isHitDebug ? Color.red : Color.green;
        Gizmos.DrawLine(debugRayStart, debugRayEnd);
        Gizmos.DrawWireSphere(debugRayStart, rayRadius);
        Gizmos.DrawWireSphere(debugRayEnd, rayRadius);
    }
}