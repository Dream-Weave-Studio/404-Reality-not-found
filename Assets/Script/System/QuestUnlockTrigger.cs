using UnityEngine;

public class QuestUnlockTrigger : MonoBehaviour
{
    public string targetQuestID;
    public GameObject spawnVFX;
    void Start() { gameObject.SetActive(false); }
    public void OnQuestCompleted(SO_Quest completedQuest)
    {
        if (completedQuest.questID != targetQuestID) return;
        gameObject.SetActive(true);
    }
}