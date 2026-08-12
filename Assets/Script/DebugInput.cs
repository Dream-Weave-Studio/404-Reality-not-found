using UnityEngine;

public class DebugInput : MonoBehaviour
{
    [Header("Riferimenti")]
    public GameEntity playerEntity; // Trascina qui il Player (Ryo)

    [Header("Oggetto di Test")]
    public SO_Item itemDiProva; // Trascina qui il Telefono o il Medikit

    void Update()
    {
        // --- GESTIONE SALUTE (Spostato da HealthUI) ---
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (playerEntity != null)
            {
                playerEntity.Heal(10);
                Debug.Log("[DEBUG] Curato 10 HP");
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (playerEntity != null)
            {
                playerEntity.TakeDamage(10);
                Debug.Log("[DEBUG] Inflitto 10 Danni");
            }
        }
        // Tasto J: Aggiunge un oggetto (per vedere se l'inventario funziona)
        /*if (Input.GetKeyDown(KeyCode.J))
        {
            if (InventoryManager.Instance != null && itemDiProva != null)
            {
                InventoryManager.Instance.AddItem(itemDiProva);
                Debug.Log($"[DEBUG] Aggiunto {itemDiProva.itemName} con tasto J");
            }
        }*/

        // Tasto K: SALVA la partita (Richiesta tua)
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
                Debug.Log("[DEBUG] Salvataggio forzato con tasto K");
            }
        }

        // Tasto L: CARICA la partita
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
                Debug.Log("[DEBUG] Caricamento forzato con tasto L");
            }
        }

        // Tasto M: Controlla cosa hai in tasca (Stampa in console)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log($"[DEBUG] Oggetti in inventario: {InventoryManager.Instance.inventory.Count}");
            foreach (var item in InventoryManager.Instance.inventory)
            {
                Debug.Log($"- {item.itemName}");
            }
        }
    }
}