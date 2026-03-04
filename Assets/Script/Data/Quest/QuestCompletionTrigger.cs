using UnityEngine;

// Metti questo componente sul GameObject da sbloccare (es. il telefono)
// Trascinalo come listener su QuestManager > OnAllRequiredCompleted nell'inspector
public class QuestCompletionTrigger : MonoBehaviour
{
    [Tooltip("Quale questID sblocca questo oggetto")]
    public string targetQuestID;

    void Start() => gameObject.SetActive(false); // parte nascosto

    public void OnQuestRequiredCompleted(SO_Quest completedQuest)
    {
        if (completedQuest.questID == targetQuestID)
        {
            gameObject.SetActive(true);
            // Opzionale: effetto spawn, particelle, audio
            Debug.Log($"[Trigger] {gameObject.name} sbloccato!");
        }
    }
}