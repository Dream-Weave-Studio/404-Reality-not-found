using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Stato Corrente")]
    // Gli oggetti che il player ha in tasca ORA
    public List<SO_Item> inventory = new List<SO_Item>();

    [Header("Database Totale")]
    // Qui devi trascinare TUTTI gli SO_Item che crei nel gioco.
    // Serve al sistema di caricamento per convertire l'ID (stringa) nell'Oggetto vero (SO).
    public List<SO_Item> allItemsDatabase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // TRUCCO: Prima di distruggermi, passo il database al sopravvissuto!
            if (this.allItemsDatabase != null && this.allItemsDatabase.Count > 0)
            {
                Instance.allItemsDatabase = this.allItemsDatabase;
            }

            Destroy(gameObject);
        }
    }

    // --- GESTIONE INVENTARIO ---

    public void AddItem(SO_Item item)
    {
        if (item == null) return;

        inventory.Add(item);
        Debug.Log($"[Inventario] Aggiunto: {item.itemName}");

        // Qui in futuro aggiornerai la UI:
        // UIManager.Instance.UpdateInventoryUI();
    }

    public void RemoveItem(string itemID)
    {
        SO_Item itemToRemove = inventory.Find(x => x.itemID == itemID);
        if (itemToRemove != null)
        {
            inventory.Remove(itemToRemove);
            Debug.Log($"[Inventario] Rimosso: {itemToRemove.itemName}");
        }
    }

    public bool HasItem(string itemID)
    {
        return inventory.Exists(x => x.itemID == itemID);
    }

    // --- SISTEMA DI SALVATAGGIO (Backend) ---

    // 1. Esporta gli ID per salvare
    public List<string> GetInventoryIDsForSave()
    {
        List<string> ids = new List<string>();
        foreach (var item in inventory)
        {
            ids.Add(item.itemID);
        }
        return ids;
    }

    // 2. Importa gli ID e ricostruisce la lista oggetti
    public void LoadInventoryFromSave(List<string> savedIDs)
    {
        inventory.Clear();

        foreach (string id in savedIDs)
        {
            // Cerca l'oggetto originale nel Database usando l'ID
            SO_Item itemFound = allItemsDatabase.Find(x => x.itemID == id);

            if (itemFound != null)
            {
                inventory.Add(itemFound);
            }
            else
            {
                Debug.LogWarning($"[Inventario] Errore Caricamento: Oggetto con ID '{id}' non trovato nel Database!");
            }
        }
    }

    // ========================================================================
    //  ZONA STRUMENTI EDITOR (Auto-Fill)
    //  Tutto ciò che sta tra #if e #endif viene ignorato quando fai la Build del gioco.
    // ========================================================================

#if UNITY_EDITOR
    // Aggiunge la voce al menu contestuale (Tasto Destro sullo Script)
    [ContextMenu("Auto-Fill Database")]
    public void AutoFillDatabase()
    {
        // 1. Pulisci la lista attuale
        allItemsDatabase.Clear();

        // 2. Cerca tutti gli SO_Item nel progetto
        string[] guids = AssetDatabase.FindAssets("t:SO_Item");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SO_Item item = AssetDatabase.LoadAssetAtPath<SO_Item>(path);

            if (item != null)
            {
                allItemsDatabase.Add(item);
            }
        }

        Debug.Log($"[InventoryManager] Database aggiornato! Trovati {allItemsDatabase.Count} oggetti.");

        // Dice a Unity che abbiamo modificato qualcosa e deve salvare
        EditorUtility.SetDirty(this);
    }
#endif
}


