using UnityEngine;

public class WallFader : MonoBehaviour
{
    private Material mat;
    private float currentHoleActivity = 0f;
    private float targetHoleActivity = 0f;

    // Timer di sicurezza: se nessuno mi colpisce, mi chiudo
    private float autoCloseTimer = 0f;

    private float openSpeed = 10f;
    private float closeSpeed = 5f;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null) mat = rend.material;
    }

    void Update()
    {
        if (mat == null) return;

        // --- LOGICA DI AUTO-CHIUSURA ---
        // Riduciamo il timer ogni frame
        autoCloseTimer -= Time.deltaTime;

        if (autoCloseTimer > 0f)
        {
            // Se il timer è attivo, significa che la camera ci sta colpendo ora
            targetHoleActivity = 1f;
        }
        else
        {
            // Se il timer è scaduto, nessuno ci sta colpendo -> CHIUDITI!
            targetHoleActivity = 0f;
        }
        // -------------------------------

        // Scegliamo la velocità in base a cosa stiamo facendo
        float smoothSpeed = (targetHoleActivity > 0.5f) ? openSpeed : closeSpeed;

        // Animazione Lerp
        if (Mathf.Abs(currentHoleActivity - targetHoleActivity) > 0.001f)
        {
            currentHoleActivity = Mathf.MoveTowards(currentHoleActivity, targetHoleActivity, smoothSpeed * Time.deltaTime);
            mat.SetFloat("_HoleActive", currentHoleActivity);
        }
    }

    // Questa funzione viene chiamata dalla Camera ogni frame in cui siamo colpiti
    public void StayOpen(Vector3 pos, float speedOpen, float speedClose)
    {
        // Resettiamo il timer: "Sono ancora colpito, rimango aperto per un altro po'"
        autoCloseTimer = 0.2f; // Basta un valore piccolo (es. 0.2s)

        openSpeed = speedOpen;
        closeSpeed = speedClose;

        // Aggiorniamo la posizione del buco
        if (mat != null)
        {
            mat.SetVector("_PlayerPos", pos);
        }
    }
}