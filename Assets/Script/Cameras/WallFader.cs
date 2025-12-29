using UnityEngine;

public class WallFader : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    // Stato
    private float currentHoleActivity = 0f;
    private float targetHoleActivity = 0f;
    private float autoCloseTimer = 0f;

    // Parametri visivi (cache)
    private float currentRadius = 1f;
    private float currentDensity = 30f;

    private float openSpeed = 10f;
    private float closeSpeed = 5f;

    // Cache degli ID Shader
    private static int HoleActiveID = Shader.PropertyToID("_HoleActive");
    private static int PlayerPosID = Shader.PropertyToID("_PlayerPos");
    private static int HoleRadiusID = Shader.PropertyToID("_HoleRadius");   // NUOVO
    private static int PixelDensityID = Shader.PropertyToID("_PixelDensity"); // NUOVO

    void Awake()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (rend == null) return;

        // 1. Logic Timer
        autoCloseTimer -= Time.deltaTime;
        targetHoleActivity = (autoCloseTimer > 0f) ? 1f : 0f;

        // 2. Animazione Alpha
        // Ottimizzazione: se siamo già stabili a 0 o 1 e i parametri non cambiano, potremmo saltare,
        // ma per il live tuning aggiorniamo sempre.
        float smoothSpeed = (targetHoleActivity > 0.5f) ? openSpeed : closeSpeed;
        currentHoleActivity = Mathf.MoveTowards(currentHoleActivity, targetHoleActivity, smoothSpeed * Time.deltaTime);

        // 3. Applica TUTTO al PropertyBlock
        rend.GetPropertyBlock(propBlock);

        propBlock.SetFloat(HoleActiveID, currentHoleActivity);

        // Passiamo costantemente anche Radius e Density per permettere il live tuning
        propBlock.SetFloat(HoleRadiusID, currentRadius);
        propBlock.SetFloat(PixelDensityID, currentDensity);

        rend.SetPropertyBlock(propBlock);
    }

    // Aggiornata per ricevere anche i parametri grafici
    public void StayOpen(Vector3 pos, float speedOpen, float speedClose, float radius, float density)
    {
        autoCloseTimer = 0.1f;
        openSpeed = speedOpen;
        closeSpeed = speedClose;

        // Salviamo i parametri grafici
        currentRadius = radius;
        currentDensity = density;

        if (rend != null)
        {
            // La posizione la aggiorniamo subito per fluidità
            rend.GetPropertyBlock(propBlock);
            propBlock.SetVector(PlayerPosID, pos);
            rend.SetPropertyBlock(propBlock);
        }
    }
}