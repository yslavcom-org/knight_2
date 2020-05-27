using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDomeController : MonoBehaviour
{

    #region Variables
    GameObject forcedFieldInstance;
    Transform refrerenceTransform;

    //this is reference to the inventory items manager managing this pool
    InventoryItemsManager inventoryItemsManager;
    bool Enabled;
    bool isActive;
    #endregion


    #region Built-in methods
    private void Start()
    {
        inventoryItemsManager = GetComponentInParent<InventoryItemsManager>();
    }

    private void Awake()
    {
        forcedFieldInstance = ReadPrefabAndCreateInstance.GetInstanceFromPrefab(HardcodedValues.StrForcedFieldDome, false);
    }

    private void LateUpdate()
    {
        if (isActive)
        {
            SetPosition();
        }
    }
    #endregion


    #region custom methods
    void SetPosition()
    {
        forcedFieldInstance.transform.position = refrerenceTransform.position;
    }

    /*
     * scaleFactor does not have effect, try to figure out later
      */
    void Activate(Transform refrerenceTransform, Vector3 scaleFactor)
    {
        if (null == forcedFieldInstance) return;

        this.refrerenceTransform = refrerenceTransform;
        SetPosition();
        forcedFieldInstance.SetActive(true);

        //this.transform.localScale = scaleFactor;
        forcedFieldInstance.transform.localScale += scaleFactor;

        isActive = true;
    }

    public void Disable()
    {
        forcedFieldInstance.SetActive(false);
        isActive = false;
    }
    
    public void InventoryManager__SetEnabled(bool enable)
    {
        this.Enabled = enable;
    }

    public bool TryUse(Transform refrerenceTransform, Vector3 scaleFactor)
    {
        bool result = false;

        if (Enabled)
        {
            int amountRequested = 1;
            int dispatched = inventoryItemsManager.RequestItemsDispatch(Iar.StackedInventory.EquipmentType.ForcefieldArmour, amountRequested);

            if (dispatched > 0)
            {
                Activate( refrerenceTransform,  scaleFactor);
                result = true;
            }
        }
        return result;
    }
    #endregion
}

