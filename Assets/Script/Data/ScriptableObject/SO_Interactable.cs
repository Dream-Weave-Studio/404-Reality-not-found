using UnityEngine;
using System.Collections.Generic; // Necessario per le List

[CreateAssetMenu(fileName = "New Interactable", menuName = "404 Reality/Interactable Data")]
public class SO_Interactable : ScriptableObject
{
    [Header("Identità")]
    public string id;
    public string displayName;
    public Sprite characterPortrait;

    [Header("Conseguenze Interazione")]
    public SO_Item itemToGive; // L'oggetto da aggiungere all'inventario
    public bool destroyAfterInteraction; // Se vero, l'oggetto sparisce dopo il click

    [Header("Contenuto Base (Default)")]
    [TextArea(3, 10)]
    public string[] dialogueLines; // Le frasi standard [cite: 7]

    [Header("Nuovo: Varianti Condizionali")]
    // Lista di varianti basate sulla memoria (es. se sai del calendario, leggi questo)
    public List<DialogueVariant> memoryVariants;

    [Header("Apprendimento")]
    [Tooltip("ID del fatto che si impara interagendo (es. 'Knows_Trash_Day')")]
    public string factToLearn;

    [Header("Requisiti & Conseguenze")]
    public SO_Item requiredItem;
    public float rageModifier;
}

// Struct per definire una variante di dialogo [cite: 7]
[System.Serializable]
public struct DialogueVariant
{
    [Tooltip("ID della memoria richiesta per sbloccare questo testo")]
    public string requiredMemoryID;

    [TextArea(3, 10)]
    public string[] alternateLines; // Array per supportare più frasi anche nelle varianti
}