using UnityEngine;

using Item = GameInventory.Item;

[RequireComponent(typeof(MyTankGame.HomingMissilePool))]
public class InventoryItemsManager : MonoBehaviour
{
    int homingMissilesAmount = 0;
    MyTankGame.HomingMissilePool homingMissilePool;

    private void Awake()
    {
        GameInventory.Slot.OnPickedItemId += OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId += OnEmptiedItemId;

        homingMissilePool = GetComponentInParent < MyTankGame.HomingMissilePool > ();

        this.name = HardcodedValues.StrInventoryItemsManagerName;


    }

    private void OnDisable()
    {
        GameInventory.Slot.OnPickedItemId -= OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId -= OnEmptiedItemId;
    }

    #region events
    private void OnPickedItemId(int id, int itemAmount)
    {
        if(HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            if (homingMissilePool != null)
            {
                //add to the homing missile count
                homingMissilesAmount = itemAmount;
                homingMissilePool.SetEnabled(0 != homingMissilesAmount);
            }
        }
    }

    private void OnEmptiedItemId(int id)
    {
        if (HardcodedValues.HomingMissilePickUp__ItemId == id)
        {
            //disable homing misile system
            if (homingMissilePool != null)
            {
                homingMissilesAmount = 0;
                homingMissilePool.SetEnabled(false);
            }
        }
    }
    #endregion
}
