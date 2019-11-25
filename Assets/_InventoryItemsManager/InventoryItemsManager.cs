using UnityEngine;

using Item = GameInventory.Item;

[RequireComponent(typeof(MyTankGame.PlayerInventoryPool))]
public class InventoryItemsManager : MonoBehaviour
{
    MyTankGame.PlayerInventoryPool playerInventoryPool;
    GameInventory.Inventory inventory;

    private void Awake()
    {
        GameInventory.Slot.OnPickedItemId += OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId += OnEmptiedItemId;

        playerInventoryPool = GetComponentInParent < MyTankGame.PlayerInventoryPool > ();
        inventory = GetComponent<GameInventory.Inventory>();

        this.name = HardcodedValues.StrInventoryItemsManagerName;


    }

    private void OnDisable()
    {
        GameInventory.Slot.OnPickedItemId -= OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId -= OnEmptiedItemId;
    }

    public int RequestItemsDispatch(int id, int amount)
    {
        if (inventory == null) return 0;

        if (HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            int dispatched = inventory.RequestItemsDispatch(id, amount);
            return dispatched;
        }

        return 0;
    }

    #region events
    private void OnPickedItemId(int id, int itemAmount)
    {
        if(HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            if (playerInventoryPool != null)
            {
                //add to the homing missile count
                playerInventoryPool.InventoryManager__SetEnabled(0 != itemAmount/*, this*/);
            }
        }
        else if(HardcodedValues.ForceFieldDomePickUp__ItemId == id)
        {
            if (playerInventoryPool != null)
            {
                //add to the homing missile count
 //               playerInventoryPool.InventoryManager__SetEnabled(0 != itemAmount, this);
            }
        }
    }

    private void OnEmptiedItemId(int id)
    {
        if (HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            //disable homing misile system
            if (playerInventoryPool != null)
            {
                playerInventoryPool.InventoryManager__SetEnabled(false/*, this*/);
            }
        }
        else if (HardcodedValues.ForceFieldDomePickUp__ItemId == id)
        {
            if (playerInventoryPool != null)
            {
                //add to the homing missile count
//                playerInventoryPool.InventoryManager__SetEnabled(0 != homingMissilesAmount, this);
            }
        }
    }
    #endregion
}
