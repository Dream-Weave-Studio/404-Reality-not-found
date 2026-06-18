using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damageAmount = 15f;
    [SerializeField] private string eventToFire = "slipped";
    [SerializeField] private Sprite characterPortrait;
    [TextArea(3, 10)]
    [SerializeField] private string[] dialogueLines = new string[] { "Ahia! Dov'è il medikit?" };

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player") && other.TryGetComponent(out GameEntity entity))
        {
            hasActivated = true;
            entity.TakeDamage(damageAmount);

            if (QuestManager.Instance != null && !string.IsNullOrEmpty(eventToFire))
            {
                QuestManager.Instance.UnlockSubsByEvent(eventToFire);
            }

            // Fallback to find portrait in the scene if not assigned
            if (characterPortrait == null)
            {
                InteractableObject[] interactables = FindObjectsOfType<InteractableObject>();
                foreach (var io in interactables)
                {
                    if (io.interactableData != null && io.interactableData.characterPortrait != null)
                    {
                        characterPortrait = io.interactableData.characterPortrait;
                        break;
                    }
                }
            }

            if (DialogManager.Instance != null && dialogueLines != null && dialogueLines.Length > 0)
            {
                DialogManager.Instance.ShowDialog(dialogueLines[0], characterPortrait);
            }
            else
            {
                Debug.Log("[DamageZone]: " + (dialogueLines != null && dialogueLines.Length > 0 ? dialogueLines[0] : ""));
            }
        }
    }
}
