using UnityEngine;

public class QuestUnlockTrigger : MonoBehaviour
{
    public string targetQuestID;
    public string targetSubObjectiveID;
    public GameObject spawnVFX;
    void Start() 
    { 
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(targetSubObjectiveID))
        {
            if (QuestManager.Instance.IsSubCompleted(targetSubObjectiveID))
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
                QuestManager.Instance.OnSubObjectiveCompleted.AddListener(OnSubObjectiveCompleted);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void OnQuestCompleted(SO_Quest completedQuest)
    {
        if (string.IsNullOrEmpty(targetSubObjectiveID))
        {
            if (completedQuest.questID != targetQuestID) return;
            gameObject.SetActive(true);
        }
    }
    private void OnSubObjectiveCompleted(string subID)
    {
        if (subID == targetSubObjectiveID)
        {
            gameObject.SetActive(true);
            if (spawnVFX != null) Instantiate(spawnVFX, transform.position, transform.rotation);
        }
    }
    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnSubObjectiveCompleted.RemoveListener(OnSubObjectiveCompleted);
        }
    }
}