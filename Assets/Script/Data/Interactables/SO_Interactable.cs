using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Interactable", menuName = "404 Reality/Interactable Data")]
public class SO_Interactable : ScriptableObject
{
    [Header("Identità")]
    public string id;
    public string displayName;
    public Sprite characterPortrait;

    [Header("Contenuto Base (Default)")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    [Header("Nuovo: Varianti Condizionali")]
    // Ora Unity troverà "DialogueVariant" perché è nel suo file pubblico separato!
    public List<DialogueVariant> memoryVariants;

    [Header("Apprendimento")]
    public string factToLearn;

    [Header("Conseguenze Interazione")]
    public SO_Item itemToGive;
    public bool destroyAfterInteraction;

    [Header("Requisiti & Conseguenze")]
    public SO_Item requiredItem;
    public float rageModifier;
}
