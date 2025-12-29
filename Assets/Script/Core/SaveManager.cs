using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFilePath;

    // Variabile temporanea per tenere i dati mentre la scena si ricarica
    private GameData pendingLoadData = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Ci iscriviamo all'evento: "Avvisami quando una scena finisce di caricare"
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    // --- SALVATAGGIO ---
    public void SaveGame()
    {
        GameData data = new GameData();

        // 1. Posizione Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition[0] = player.transform.position.x;
            data.playerPosition[1] = player.transform.position.y;
            data.playerPosition[2] = player.transform.position.z;
        }

        // 2. Memoria (Usa i nomi corretti del tuo MemoryManager)
        if (MemoryManager.Instance != null)
        {
            data.learnedFacts = MemoryManager.Instance.GetFactsForSave();
        }

        // 3. Inventario (Usa i nomi corretti del tuo InventoryManager)
        if (InventoryManager.Instance != null)
        {
            data.inventoryItemIDs = InventoryManager.Instance.GetInventoryIDsForSave();
        }

        // 4. Stato Intro
        if (GameManager.Instance != null)
        {
            data.introFinished = GameManager.Instance.introFinished;
        }

        // 5. Obiettivo
        if (ObjectiveManager.Instance != null)
        {
            data.currentObjective = ObjectiveManager.Instance.GetCurrentObjective();
        }

        // Scrittura su file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"[SaveManager] Salvato in: {saveFilePath}");
    }

    // --- CARICAMENTO ---
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("[SaveManager] Nessun file di salvataggio trovato.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);

        // FASE 1: Leggi i dati in memoria ma NON applicarli ancora
        pendingLoadData = JsonUtility.FromJson<GameData>(json);

        // FASE 2: Ricarica la scena (questo resetterà tutti gli oggetti)
        // I dati restano al sicuro nella variabile 'pendingLoadData'
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Questo metodo parte AUTOMATICAMENTE quando la scena ha finito di ricaricarsi
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se non ci sono dati in attesa (es. avvio normale del gioco), non fare nulla
        if (pendingLoadData == null) return;

        Debug.Log("[SaveManager] Scena ricaricata. Applico i dati salvati...");

        // FASE 3: Applicazione Dati (Ora la scena è pulita e pronta)

        // A. Ripristina Intro
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestoreGameState(pendingLoadData.introFinished);
        }

        // B. Ripristina Posizione Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false; // Disabilita physics per teletrasporto sicuro

            player.transform.position = new Vector3(
                pendingLoadData.playerPosition[0],
                pendingLoadData.playerPosition[1],
                pendingLoadData.playerPosition[2]
            );

            if (cc != null) cc.enabled = true;
        }

        // C. Ripristina Inventario (Usa inventoryItemIDs come nel tuo GameData)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadInventoryFromSave(pendingLoadData.inventoryItemIDs);
        }

        // D. Ripristina Memoria (Usa learnedFacts come nel tuo GameData)
        if (MemoryManager.Instance != null)
        {
            MemoryManager.Instance.LoadFactsFromSave(pendingLoadData.learnedFacts);
        }

        // E. Ripristina Obiettivo
        if (ObjectiveManager.Instance != null && !string.IsNullOrEmpty(pendingLoadData.currentObjective))
        {
            ObjectiveManager.Instance.SetObjective(pendingLoadData.currentObjective);
        }

        // Pulizia finale
        pendingLoadData = null;
    }
}