using UnityEngine;

public class WallFader : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    private float currentHoleActivity = 0f;
    private float targetHoleActivity = 0f;
    private float autoCloseTimer = 0f;

    private float currentRadius = 1f;
    private float currentDensity = 30f;

    private float openSpeed = 10f;
    private float closeSpeed = 5f;

    private static int HoleActiveID = Shader.PropertyToID("_HoleActive");
    private static int PlayerPosID = Shader.PropertyToID("_PlayerPos");
    private static int HoleRadiusID = Shader.PropertyToID("_HoleRadius");   
    private static int PixelDensityID = Shader.PropertyToID("_PixelDensity");

    void Awake()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (rend == null) return;

        autoCloseTimer -= Time.deltaTime;
        targetHoleActivity = (autoCloseTimer > 0f) ? 1f : 0f;

        if (currentHoleActivity <= 0.001f && targetHoleActivity == 0f)
        {
            if (currentHoleActivity != 0f)
            {
                currentHoleActivity = 0f;
                rend.GetPropertyBlock(propBlock);
                propBlock.SetFloat(HoleActiveID, 0f);
                rend.SetPropertyBlock(propBlock);
            }
            return;
        }

        float smoothSpeed = (targetHoleActivity > 0.5f) ? openSpeed : closeSpeed;
        currentHoleActivity = Mathf.MoveTowards(currentHoleActivity, targetHoleActivity, smoothSpeed * Time.deltaTime);

        rend.GetPropertyBlock(propBlock);

        propBlock.SetFloat(HoleActiveID, currentHoleActivity);

        propBlock.SetFloat(HoleRadiusID, currentRadius);
        propBlock.SetFloat(PixelDensityID, currentDensity);

        rend.SetPropertyBlock(propBlock);
    }

    public void StayOpen(Vector3 pos, float speedOpen, float speedClose, float radius, float density)
    {
        autoCloseTimer = 0.1f;
        openSpeed = speedOpen;
        closeSpeed = speedClose;

        currentRadius = radius;
        currentDensity = density;

        if (rend != null)
        {
            rend.GetPropertyBlock(propBlock);
            propBlock.SetVector(PlayerPosID, pos);
            rend.SetPropertyBlock(propBlock);
        }
    }
}
