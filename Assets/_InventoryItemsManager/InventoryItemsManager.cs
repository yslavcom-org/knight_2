using UnityEngine;

using Item = GameInventory.Item;

[RequireComponent(typeof(MyTankGame.HomingMissilePool))]
public class InventoryItemsManager : MonoBehaviour
{
    GameInventory.Inventory inventory;

    //various assets possible
    [SerializeField]
    int homingMissilesAmount = 0;
    MyTankGame.HomingMissilePool homingMissilePool;

    [SerializeField]
    int forceFieldDomeAmount = 0;
    ForceFieldDomeController forceFieldDomeController;

    private void Awake()
    {
        GameInventory.Slot.OnPickedItemId += OnPickedItemId;
        GameInventory.Slot.OnEmptiedItemId += OnEmptiedItemId;

        inventory = GetComponent<GameInventory.Inventory>();

        homingMissilePool = GetComponentInParent < MyTankGame.HomingMissilePool > ();
        forceFieldDomeController = GetComponentInParent<ForceFieldDomeController>();

        this.name = HardcodedValues.StrInventoryItemsManagerName;
    }

    bool playerIdRetrieved = false;
    int GetThisPlayerId()
    {
        var objectId = GetComponentInParent<MyTankGame.IObjectId>();
        int playerId = objectId.GetId();
        playerIdRetrieved = true;

        return playerId;
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
        else if (HardcodedValues.ForcedFieldDomePickUp__ItemId == id)
        {
            int dispatched = inventory.RequestItemsDispatch(id, amount);
            return dispatched;
        }
        else if (HardcodedValues.HealthPackPickUp__ItemId == id)
        {
            int dispatched = inventory.RequestItemsDispatch(id, amount);
            return dispatched;
        }
        

        return 0;
    }

    #region events
    private void OnPickedItemId(int playerId, int itemId, int itemAmount)
    {
        int this_playerId = GetThisPlayerId();
        if (this_playerId != playerId) return;

        if(HardcodedValues.HomingMissilePickUp__ItemId == itemId)
        {
            if (homingMissilePool != null)
            {
                //add to the homing missile count
                homingMissilesAmount = itemAmount;
                homingMissilePool.InventoryManager__SetEnabled(0 != homingMissilesAmount);
            }
        }
        else if (HardcodedValues.ForcedFieldDomePickUp__ItemId == itemId)
        {
            if (forceFieldDomeController != null)
            {
                //add to the forced field count
                forceFieldDomeAmount = itemAmount;
                forceFieldDomeController.InventoryManager__SetEnabled(0 != forceFieldDomeAmount);
            }
        }
        else if (HardcodedValues.HealthPackPickUp__ItemId == itemId)
        {
        }
    }

    private void OnEmptiedItemId(int playerId, int itemId)
    {
        int this_playerId = GetThisPlayerId();
        if (this_playerId != playerId) return;

        if (HardcodedValues.HomingMissilePickUp__ItemId == itemId)
        {
            //disable homing misile system
            if (homingMissilePool != null)
            {
                homingMissilesAmount = 0;
                homingMissilePool.InventoryManager__SetEnabled(false);
            }
        }
        else if (HardcodedValues.ForcedFieldDomePickUp__ItemId == itemId)
        {
            if (forceFieldDomeController != null)
            {
                //add to the homing missile count
                forceFieldDomeAmount = 0;
                forceFieldDomeController.InventoryManager__SetEnabled(false);
            }
        }
        else if (HardcodedValues.HealthPackPickUp__ItemId == itemId)
        {
        }
    }
    #endregion
}
