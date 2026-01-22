using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reazione generica per Accendere/Spegnere oggetti.
/// </summary>
public class ToggleActiveReaction : InteractionReaction
{
    [Tooltip("L'oggetto da attivare/disattivare.")]
    public GameObject targetObject;

    public enum ToggleMode { Toggle, TurnOn, TurnOff }
    public ToggleMode mode = ToggleMode.Toggle;

    protected override void PerformReaction(GameObject interactor)
    {
        if (targetObject == null) return;

        bool newState = false;
        switch (mode)
        {
            case ToggleMode.Toggle:
                newState = !targetObject.activeSelf;
                break;
            case ToggleMode.TurnOn:
                newState = true;
                break;
            case ToggleMode.TurnOff:
                newState = false;
                break;
        }

        targetObject.SetActive(newState);
    }
}
