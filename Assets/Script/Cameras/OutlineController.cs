using UnityEngine;
[ExecuteInEditMode]
public class OutlineController : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 0.1f)]
    public float width = 0.02f;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private float _lastWidth = -1f; // Valore impossibile come default

    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        ApplyOutline(); // Applica subito all'attivazione
    }

    private void Update()
    {
        // Aggiorna SOLO se il valore è cambiato
        if (Mathf.Approximately(width, _lastWidth)) return;
        ApplyOutline();
    }

    private void ApplyOutline()
    {
        if (_renderer == null) return;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_Outline_Thickness", width);
        _renderer.SetPropertyBlock(_propBlock);

        _lastWidth = width;
    }
}