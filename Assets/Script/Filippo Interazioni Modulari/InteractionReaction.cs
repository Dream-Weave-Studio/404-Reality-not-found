using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe base abstract per tutte le reazioni.
/// Mettendo questo script (o i suoi figli) sullo stesso oggetto dell'InteractableObject,
/// verranno chiamati automaticamente quando il giocatore interagisce.
/// </summary>
public abstract class InteractionReaction : MonoBehaviour
{
    // Ritardo opzionale per sequenziare effetti (es. prima suono, poi rotazione)
    [Tooltip("Aspetta X secondi prima di eseguire la reazione.")]
    public float delay = 0f;

    /// <summary>
    /// Metodo pubblico chiamato dall'InteractableObject.
    /// </summary>
    public void React(GameObject interactor)
    {
        if (delay > 0)
        {
            StartCoroutine(ReactDelayed(interactor));
        }
        else
        {
            PerformReaction(interactor);
        }
    }

    private System.Collections.IEnumerator ReactDelayed(GameObject interactor)
    {
        yield return new WaitForSeconds(delay);
        PerformReaction(interactor);
    }

    /// <summary>
    /// La logica specifica della reazione (ruota, suona, esplodi, etc.)
    /// </summary>
    protected abstract void PerformReaction(GameObject interactor);
}