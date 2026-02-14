using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject itemSlotPrefab;

    private void Start()
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        RefreshUI(); // popola la UI all'avvio
    }

    private void RefreshUI()
    {
        // Pulire tutti gli slot precedenti
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Creare uno slot per ogni item
        foreach (var item in InventoryManager.Instance.inventory)
        {
            GameObject slot = Instantiate(itemSlotPrefab, contentParent);
            slot.GetComponent<ItemSlotUI>().Setup(item);
        }
    }
}
