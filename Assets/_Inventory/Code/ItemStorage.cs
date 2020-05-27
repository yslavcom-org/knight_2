using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameInventory
{
    public class ItemStorage : MonoBehaviour
    {
        public static event Action<int, Iar.StackedInventory.EquipmentType, int> OnPickedItemId = delegate { }; // notify the newly picked item id
        public static event Action<int, Iar.StackedInventory.EquipmentType> OnEmptiedItemId = delegate { }; // notify the item id which became emptied

        public GameObject item;

        private bool empty=true;
        public Iar.StackedInventory.EquipmentType EquipmentType;
        public int amount=0;//this is the quantity of this item

        int playerId;

        public void SetId(int id)
        {
            playerId = id;
        }

        private void Start()
        {
            var objectId = GetComponentInParent<MyTankGame.IObjectId>();
            if (null == objectId)
            {
                Debug.Log("missing parent");
            }
            else
            {
                playerId = objectId.GetId();
            }

        }

         public void UpdateSlotBusy()
        {
            empty = false;

            OnPickedItemId(playerId, EquipmentType, amount);
        }

        public bool IfSlotBusy()
        {
            return !empty;
        }

        public int UseItem(int amount)
        {
            int dispatched = 0;
            if (!empty)
            {
                if(this.amount <= amount)
                {
                    dispatched = this.amount;
                    EmptyItem();
                }
                else
                {
                    this.amount -= amount;
                    dispatched = amount;
                }
            }

            return dispatched;
        }

        private void EmptyItem()
        {
            OnEmptiedItemId(playerId, EquipmentType);

            amount = 0;

            empty = true;
        }
    }
}
