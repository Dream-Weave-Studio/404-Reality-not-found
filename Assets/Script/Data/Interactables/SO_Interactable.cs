using UnityEngine;

[CreateAssetMenu(fileName = "New Interactable", menuName = "404 Reality/Interactable Data")]
public class SO_Interactable : ScriptableObject
{
    [Header("Identità")]
    public string id;                // Es. "frigo_cucina"
    public string displayName;       // Es. "Frigo Vuoto"
    public Sprite characterPortrait; // Il volto di chi parla(Ryo, Dino, ecc.)

    [Header("Contenuto Narrativo")]
    [TextArea(3, 10)]
    public string[] dialogueLines;  // Le frasi che Ryo dice (Es. "Il latte è scaduto...")

    [Header("Requisiti")]
    public SO_Item requiredItem;    // Serve un oggetto per interagire? (Es. Chiavi per la porta)

    [Header("Conseguenze")]
    public float rageModifier;      // Quanta rabbia aggiunge? (Es. +10 se il frigo è vuoto)
}