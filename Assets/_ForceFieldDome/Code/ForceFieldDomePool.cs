using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDomePool : MonoBehaviour
{
    #region Var
    //this is reference to the inventory items manager managing this pool
    InventoryItemsManager inventoryItemsManager;
    bool enabled;
    #endregion

    #region built-in methods
    // Start is called before the first frame update
    void Start()
    {
        inventoryItemsManager = GetComponentInParent<InventoryItemsManager>();
    }
    #endregion

    #region custom methods
    public void InventoryManager__SetEnabled(bool enable)
    {
        this.enabled = enable;
    }

    public bool TryUse()
    {
        if (enabled)
        {
            int amountRequested = 1;
            int dispatched = inventoryItemsManager.RequestItemsDispatch(Iar.StackedInventory.EquipmentType.ForcefieldArmour, amountRequested);

            if(dispatched > 0)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}
