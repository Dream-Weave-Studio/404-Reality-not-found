using UnityEngine;
using System.IO; // Necessario per lavorare con i file

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Definiamo il percorso del file. Su Windows sarà in AppData/LocalLow/TuoNome/404Reality...
            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else Destroy(gameObject);
    }

    // Chiamalo quando vuoi salvare (es. ai Checkpoint o dal Menu)
    public void SaveGame()
    {
        GameData data = new GameData();

        // 1. Raccogli Posizione Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition[0] = player.transform.position.x;
            data.playerPosition[1] = player.transform.position.y;
            data.playerPosition[2] = player.transform.position.z;
        }

        // 2. Raccogli Memoria
        if (MemoryManager.Instance != null)
        {
            data.learnedFacts = MemoryManager.Instance.GetFactsForSave();
        }

        // 3. Raccogli Inventario
        if (InventoryManager.Instance != null)
        {
            data.inventoryItemIDs = InventoryManager.Instance.GetInventoryIDsForSave();
        }

        // 4. Salva stato Intro
        if (GameManager.Instance != null)
        {
            data.introFinished = GameManager.Instance.introFinished;
        }

        // 5. Salva Obiettivo Corrente (NUOVO)
        if (ObjectiveManager.Instance != null)
        {
            data.currentObjective = ObjectiveManager.Instance.GetCurrentObjective();
        }

        // 6. Scrivi su disco
        string json = JsonUtility.ToJson(data, true); // 'true' per formattazione leggibile
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"[SaveManager] Partita salvata in: {saveFilePath}");
    }

    // Chiamalo quando premi "Continua"
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("[SaveManager] Nessun file di salvataggio trovato.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // 1. Ripristina Posizione
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 pos = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            // Se usi un CharacterController, disabilitalo momentaneamente prima di teletrasportare
            player.transform.position = pos;
        }

        // 2. Ripristina Memoria
        if (MemoryManager.Instance != null)
        {
            MemoryManager.Instance.LoadFactsFromSave(data.learnedFacts);
        }

        // 3. Ripristina Inventario
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadInventoryFromSave(data.inventoryItemIDs);
        }

        // 4. Ripristina stato Intro 
        if (GameManager.Instance != null)
        {
            // Questo setterà lo stato corretto e bloccherà l'intro se necessario
            GameManager.Instance.RestoreGameState(data.introFinished);
        }

        // 5. Ripristina Obiettivo (NUOVO)
        // IMPORTANTE: Questo deve avvenire DOPO aver ripristinato lo stato Intro
        if (ObjectiveManager.Instance != null)
        {
            // Se nel salvataggio c'è un obiettivo, usalo. 
            // Se è vuoto (vecchio save), usa un default.
            if (!string.IsNullOrEmpty(data.currentObjective))
            {
                ObjectiveManager.Instance.SetObjective(data.currentObjective);
            }
        }

        Debug.Log("[SaveManager] Partita caricata con successo.");
    }
}