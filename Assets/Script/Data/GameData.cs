using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool introFinished;
    public float[] playerPosition;
    public List<string> learnedFacts;
    public List<string> inventoryItemIDs;

    // Quest System
    public string activeQuestID;
    public List<string> activeStackIDs;       // missioni attive nello stack
    public List<string> completedSubIDs;      // missioni completate

    public GameData()
    {
        playerPosition = new float[3];
        learnedFacts = new List<string>();
        inventoryItemIDs = new List<string>();
        activeStackIDs = new List<string>();
        completedSubIDs = new List<string>();
        introFinished = false;
        activeQuestID = "trova_telefono";
    }
}