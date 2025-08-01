using UnityEngine;
using UnityEngine.Events;

public class GenericInteractable : MonoBehaviour , IInteractable
{
    public UnityEvent OnInteract;

    public void Interact()
    {
        OnInteract.Invoke();
    }
}
