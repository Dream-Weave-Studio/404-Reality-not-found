using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddItem : MonoBehaviour
{
    public SO_Item testItem;

    void Update()
    {
        // Premi T per aggiungere l'item all'inventario
        if (Input.GetKeyDown(KeyCode.T))
        {
            InventoryManager.Instance.AddItem(testItem);
        }
    }
}
