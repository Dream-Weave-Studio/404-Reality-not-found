using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "404 Reality/Item Data")]
public class SO_Item : ScriptableObject
{
    [Header("Info Generali")]
    public string itemID;
    public string itemName;

    [Header("Visual")]
    public Sprite icon;
    public GameObject prefab;

    [Header("Dettagli")]
    [TextArea(3, 10)]
    public string description;

    [Header("Logica")]
    public bool isConsumable;
    public bool isKeyItem;
    public bool isStackable;
}