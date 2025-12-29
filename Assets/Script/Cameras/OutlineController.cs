using UnityEngine;

[ExecuteInEditMode] // Ti permette di vedere le modifiche senza premere Play!
public class OutlineController : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 0.1f)]
    public float width = 0.02f; // Valore di default

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        // Ottimizzazione: Eseguiamo questo codice solo se il renderer esiste
        if (_renderer == null) return;

        // 1. Recupera i valori attuali dell'oggetto (per non sovrascrivere altre cose)
        _renderer.GetPropertyBlock(_propBlock);

        // 2. Imposta il nuovo valore di larghezza
        _propBlock.SetFloat("_Outline_Thickness", width);

        // 3. Applica il blocco modificato al renderer
        _renderer.SetPropertyBlock(_propBlock);
    }
}