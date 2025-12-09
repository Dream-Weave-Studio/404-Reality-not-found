using UnityEngine;

public class CameraObstructionFader : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public LayerMask wallLayer;

    [Header("Settings")]
    public float fadeSpeed = 10f;     // Velocità apertura
    public float closeSpeed = 5f;     // Velocità chiusura
    public float radius = 0.2f;
    public float heightOffset = 1.5f;

    void Update()
    {
        if (player == null) return;

        // Calcoliamo la posizione ideale del buco (Tunnel)
        Vector3 targetPos = player.position + Vector3.up * heightOffset;

        Vector3 dir = targetPos - transform.position;
        float dist = dir.magnitude;

        // SphereCast per trovare I MURI che coprono la visuale
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, dir, dist, wallLayer);

        // Per ogni muro colpito, diciamo solo "RIMANI APERTO"
        foreach (RaycastHit hit in hits)
        {
            WallFader fader = hit.collider.GetComponent<WallFader>();

            // Se il muro non ha lo script, glielo mettiamo noi
            if (fader == null) fader = hit.collider.gameObject.AddComponent<WallFader>();

            // Chiamiamo la funzione StayOpen. 
            // Se smettiamo di chiamarla (perché il raggio non colpisce più),
            // il muro si chiuderà da solo grazie al suo timer interno.
            fader.StayOpen(targetPos, fadeSpeed, closeSpeed);
        }
    }
}