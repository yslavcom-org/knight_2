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
        GameInventory.ItemStorage.OnPickedItemId += OnPickedItemId;
        GameInventory.ItemStorage.OnEmptiedItemId += OnEmptiedItemId;

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
        GameInventory.ItemStorage.OnPickedItemId -= OnPickedItemId;
        GameInventory.ItemStorage.OnEmptiedItemId -= OnEmptiedItemId;
    }

    public int RequestItemsDispatch(Iar.StackedInventory.EquipmentType EquipmentType, int amount)
    {
        if (inventory == null) return 0;

        if (Iar.StackedInventory.EquipmentType.HomingMissile == EquipmentType)
        {
            int dispatched = inventory.RequestItemsDispatch(EquipmentType, amount);
            return dispatched;
        }
        else if (Iar.StackedInventory.EquipmentType.ForcefieldArmour == EquipmentType)
        {
            int dispatched = inventory.RequestItemsDispatch(EquipmentType, amount);
            return dispatched;
        }
        else if (Iar.StackedInventory.EquipmentType.Health == EquipmentType)
        {
            int dispatched = inventory.RequestItemsDispatch(EquipmentType, amount);
            return dispatched;
        }
        

        return 0;
    }

    #region events
    private void OnPickedItemId(int playerId, Iar.StackedInventory.EquipmentType EquipmentType, int itemAmount)
    {
        int this_playerId = GetThisPlayerId();
        if (this_playerId != playerId) return;

        if(Iar.StackedInventory.EquipmentType.HomingMissile == EquipmentType)
        {
            if (homingMissilePool != null)
            {
                //add to the homing missile count
                homingMissilesAmount = itemAmount;
                homingMissilePool.InventoryManager__SetEnabled(0 != homingMissilesAmount);
            }
        }
        else if (Iar.StackedInventory.EquipmentType.ForcefieldArmour == EquipmentType)
        {
            if (forceFieldDomeController != null)
            {
                //add to the forced field count
                forceFieldDomeAmount = itemAmount;
                forceFieldDomeController.InventoryManager__SetEnabled(0 != forceFieldDomeAmount);
            }
        }
        else if (Iar.StackedInventory.EquipmentType.Health == EquipmentType)
        {//do nothing
        }
    }

    private void OnEmptiedItemId(int playerId, Iar.StackedInventory.EquipmentType EquipmentType)
    {
        int this_playerId = GetThisPlayerId();
        if (this_playerId != playerId) return;

        if (Iar.StackedInventory.EquipmentType.HomingMissile == EquipmentType)
        {
            //disable homing misile system
            if (homingMissilePool != null)
            {
                homingMissilesAmount = 0;
                homingMissilePool.InventoryManager__SetEnabled(false);
            }
        }
        else if (Iar.StackedInventory.EquipmentType.ForcefieldArmour == EquipmentType)
        {
            if (forceFieldDomeController != null)
            {
                //add to the homing missile count
                forceFieldDomeAmount = 0;
                forceFieldDomeController.InventoryManager__SetEnabled(false);
            }
        }
        else if (Iar.StackedInventory.EquipmentType.Health == EquipmentType)
        {//do nothing
        }
    }
    #endregion
}
