using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // 1. Stato dell'intro
    public bool introFinished;
    
    // 2. Dove si trova il giocatore
    public float[] playerPosition; // Unity non salva Vector3 in JSON, usiamo float array [x, y, z]

    // 3. Cosa sa il giocatore (MemoryManager)
    // Salviamo solo le chiavi (IDs) dei fatti scoperti. 
    // Se è nella lista, è true.
    public List<string> learnedFacts;

    // 4. Cosa possiede il giocatore (InventoryManager)
    // Salviamo solo gli ID degli oggetti (es. "medikit", "telefono")
    public List<string> inventoryItemIDs;

    // 5. Salviamo il testo dell'obiettivo corrente
    public string currentObjective;


    // Costruttore: inizializza le liste per evitare errori null
    public GameData()
    {
        playerPosition = new float[3];
        learnedFacts = new List<string>();
        inventoryItemIDs = new List<string>();
        introFinished = false;

        // Default se è una nuova partita
        currentObjective = "TROVA IL TELEFONO";
    }
}