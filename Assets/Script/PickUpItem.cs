public class PickUpItem : InteractableObject
{
    public SO_Item testItem;

    public override void Interact()
    {
        base.Interact();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(testItem);
        }

        Destroy(gameObject);
    }
}
