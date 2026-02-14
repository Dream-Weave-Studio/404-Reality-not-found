using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button removeButton;

    private SO_Item currentItem;

    // Inizializza lo slot con i dati dell'item
    public void Setup(SO_Item item)
    {
        currentItem = item;

        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
        }

        if (nameText != null)
            nameText.text = item.itemName;

        if (tooltipPanel != null && descriptionText != null)
        {
            descriptionText.text = item.description;
            tooltipPanel.SetActive(false); // nascosto di default
        }

        if (removeButton != null)
        {
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(OnRemoveButtonClicked);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    private void OnRemoveButtonClicked()
    {
        if (currentItem != null)
        {
            InventoryManager.Instance.RemoveItem(currentItem.itemID);
        }
    }
}
