using UnityEngine;

// Questo attributo ci permette di creare nuovi oggetti direttamente dal menu di Unity
[CreateAssetMenu(fileName = "New Item", menuName = "404 Reality/Item Data")]
public class SO_Item : ScriptableObject
{
    [Header("Info Generali")]
    public string itemID;             // ID univoco (es. "medikit_01")
    public string itemName;       // Nome visualizzato (es. "Medikit")

    [Header("Visual")]
    public Sprite icon;           // Icona per l'UI dell'inventario
    public GameObject prefab;     // Modello 3D (opzionale, se serve spawnarlo)

    [Header("Dettagli")]
    [TextArea(3, 10)]
    public string description;    // Descrizione (utile per l'esame degli oggetti)

    [Header("Logica")]
    public bool isConsumable;     // Se vero, si consuma all'uso (es. Medikit)
    public bool isKeyItem;        // Se vero, non può essere scartato (es. Telefono)
    public bool isStackable;      // Se vero, si può accumulare (es. 3 monete)
}