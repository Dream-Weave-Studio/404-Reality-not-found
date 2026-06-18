using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddItem : InteractableObject
{
    public SO_Item testItem;

    public override void Interact()
    {
        // Chiama eventuale logica base (se serve dialogo ecc.)
        base.Interact();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(testItem);
        }

        Destroy(gameObject);
    }
}
