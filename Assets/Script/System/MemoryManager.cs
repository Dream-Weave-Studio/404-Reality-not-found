using UnityEngine;
using System.Collections.Generic;

// Gestisce i fatti scoperti (es. "Knows_Trash_Schedule")
public class MemoryManager : MonoBehaviour
{
    public static MemoryManager Instance;

    // Dizionario: ID Fatto -> Vero/Falso
    private Dictionary<string, bool> memory = new Dictionary<string, bool>();

    [Header("Debug")]
    [SerializeField] private List<string> debugKnownFacts = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Impariamo qualcosa di nuovo
    public void SetFact(string factID)
    {
        if (string.IsNullOrEmpty(factID)) return;

        if (!memory.ContainsKey(factID))
        {
            memory.Add(factID, true);
            debugKnownFacts.Add(factID); // Per vederlo nell'inspector
            Debug.Log($"[Memory] Nuova info acquisita: {factID}");
        }
    }

    // Controlliamo se sappiamo qualcosa
    public bool CheckFact(string factID)
    {
        if (string.IsNullOrEmpty(factID)) return false;
        return memory.ContainsKey(factID) && memory[factID];
    }

    // Metodo per ESPORTARE i dati (Save)
    public List<string> GetFactsForSave()
    {
        // Convertiamo le chiavi del dizionario in una lista
        return new List<string>(memory.Keys);
    }

    // Metodo per IMPORTARE i dati (Load)
    public void LoadFactsFromSave(List<string> loadedFacts)
    {
        memory.Clear(); // Puliamo la memoria attuale
        debugKnownFacts.Clear();

        foreach (string factID in loadedFacts)
        {
            if (!memory.ContainsKey(factID))
            {
                memory.Add(factID, true);
                debugKnownFacts.Add(factID);
            }
        }
    }
}