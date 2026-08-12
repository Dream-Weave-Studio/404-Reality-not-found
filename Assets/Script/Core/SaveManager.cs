using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private string saveFilePath;
    private GameData pendingLoadData = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    public void SaveGame()
    {
        GameData data = new GameData();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition[0] = player.transform.position.x;
            data.playerPosition[1] = player.transform.position.y;
            data.playerPosition[2] = player.transform.position.z;
        }

        if (MemoryManager.Instance != null) data.learnedFacts = MemoryManager.Instance.GetFactsForSave();
        if (InventoryManager.Instance != null) data.inventoryItemIDs = InventoryManager.Instance.GetInventoryIDsForSave();
        if (GameManager.Instance != null) data.introFinished = GameManager.Instance.introFinished;

        if (QuestManager.Instance != null)
        {
            SO_Quest active = QuestManager.Instance.GetActiveQuest();
            if (active != null)
            {
                data.activeQuestID = active.questID;
                data.activeStackIDs = QuestManager.Instance.GetActiveStackForSave();
                data.completedSubIDs = QuestManager.Instance.GetCompletedSubsForSave();
            }
        }

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data, true));
        Debug.Log("[SaveManager] Salvato in: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("[SaveManager] Nessun salvataggio trovato.");
            return;
        }
        pendingLoadData = JsonUtility.FromJson<GameData>(File.ReadAllText(saveFilePath));
        if (GameManager.Instance != null)
            GameManager.Instance.RestoreGameState(pendingLoadData.introFinished);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pendingLoadData == null) return;
        Debug.Log("[SaveManager] Applico i dati salvati...");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.SetBool("SkipIntro", false);
            if (cc != null) cc.enabled = false;
            player.transform.position = new Vector3(
                pendingLoadData.playerPosition[0],
                pendingLoadData.playerPosition[1],
                pendingLoadData.playerPosition[2]);
            if (cc != null) cc.enabled = true;
        }

        if (InventoryManager.Instance != null) InventoryManager.Instance.LoadInventoryFromSave(pendingLoadData.inventoryItemIDs);
        if (MemoryManager.Instance != null) MemoryManager.Instance.LoadFactsFromSave(pendingLoadData.learnedFacts);

        if (QuestManager.Instance != null && !string.IsNullOrEmpty(pendingLoadData.activeQuestID))
        {
            QuestManager.Instance.LoadQuestFromSave(
                pendingLoadData.activeQuestID,
                pendingLoadData.activeStackIDs,
                pendingLoadData.completedSubIDs
            );
        }

        pendingLoadData = null;
    }
}