using UnityEditor;
using UnityEngine;

/*
HomingMissilePool which will dispatch the missile Launch.
Objects get created on start-up and are reused
*/

namespace MyTankGame
{
    using Item = GameInventory.Item;
    public class HomingMissilePool : MonoBehaviour
    {
        public const int homingMissileCount = 10;
        GameObject[] homingMissilePool;
        private int currentIdx = -1;
        public bool Enabled { get; private set; }

        //this is reference to the inventory items manager managing this pool
        InventoryItemsManager inventoryItemsManager;

        // Start is called before the first frame update
        void Start()
        {
            inventoryItemsManager = GetComponentInParent<InventoryItemsManager>();


            var homingMissilePrefab = Instantiate(Resources.Load(HardcodedValues.StrResource_HomingMissile));
            //var homingMissilePrefab = ReadPrefabAndCreateInstance.GetPrefab(HardcodedValues.StrHomingMissile);
            if (homingMissilePrefab == null) return;

            homingMissilePool = new GameObject[homingMissileCount];
            if (null == homingMissilePool) return;

            for (int i = 0; i < homingMissileCount; i++)
            {
                homingMissilePool[i] = Instantiate(homingMissilePrefab) as GameObject;
            }
            currentIdx = 0;
        }

        #region Custom methods
        int GetTotalObjects(int amountRequested, out int amountDispatched)
        {
            //is the homing missile pool functional at all
            if (null == homingMissilePool)
            {
                amountDispatched = 0;
                return 0;
            }

            //request items from the inventory managers
            if(inventoryItemsManager == null)
            {
                amountDispatched = 0;
                return 0;
            }

            int dispatched = inventoryItemsManager.RequestItemsDispatch(HardcodedValues.HomingMissilePickUp__ItemId, amountRequested);
            if(dispatched == 0)
            {
                amountDispatched = 0;
                return 0;
            }

            amountDispatched = dispatched;
            return homingMissilePool.Length;
        }

        int GetNextIdleObjectIdx(int amountRequested, out int amountDispatched)
        {
            int totalObj = GetTotalObjects(amountRequested, out int dispatched);
            if (totalObj == 0 
                || dispatched == 0)
            {
                amountDispatched = 0;
                return -1;
            }
            else
            {
                //homing miisle pool is functional and inventory dispatched items
                if (currentIdx >= totalObj)
                {
                    currentIdx = 0;
                }

                amountDispatched = dispatched;
                return currentIdx++;
            }
        }

        public void InventoryManager__SetEnabled(bool enable)
        {
            Enabled = enable;
        }

        public bool TryUseHomingMissile(out GameObject homingMissile)
        {
            if (Enabled)
            {
                int amount_to_request = 1;
                int idx = GetNextIdleObjectIdx(amount_to_request, out int amount_dispatched); /* amount of objects to use*/
                if (0 <= idx)
                {
                    var obj = homingMissilePool[idx];
                    homingMissile = obj;
                    return true;
                }
            }

            homingMissile = null;
            return false;
        }

        #endregion
    }
}
