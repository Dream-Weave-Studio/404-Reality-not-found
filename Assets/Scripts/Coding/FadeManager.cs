using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;   // immagine nera a schermo intero
    [SerializeField] private float fadeDuration = 1f; // tempo dissolvenza

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Per immagine non assegnata, tenta automaticamente
        if (fadeImage == null)
        {
            fadeImage = GetComponentInChildren<Image>();
        }
    }

    // Dissolvenza verso nero
    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    // Dissolvenza da nero a trasparente
    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadeImage.color = c;
    }
}
