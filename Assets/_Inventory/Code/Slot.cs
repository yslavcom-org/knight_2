using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameInventory
{
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        public static event Action<int, int, int> OnPickedItemId = delegate { }; // notify the newly picked item id
        public static event Action<int, int> OnEmptiedItemId = delegate { }; // notify the item id which became emptied

        public GameObject item;

        private bool empty=true;
        public int id;
        public string type;
        public string description;
        public int amount=0;//this is the quantity of this item

        public Transform slotIconGO;
        public Sprite icon;

        int playerId;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (!empty)
            {
                //display data about this item
            }
        }

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

            if (0 != transform.childCount)
            {
                slotIconGO = transform.GetChild(0);
            }

            if (icon != null)
            {
                AssignIconToSlot();
            }
        }

        void AssignIconToSlot()
        {
            if (slotIconGO != null)
            {
                slotIconGO.GetComponent<Image>().sprite = icon;
            }
        }

        public void UpdateSlotBusy()
        {
            AssignIconToSlot();
            empty = false;

            OnPickedItemId(playerId, id, amount);
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
            OnEmptiedItemId(playerId, id);

            amount = 0;
            icon = null;
            if (slotIconGO != null)
            {
                slotIconGO.GetComponent<Image>().sprite = null;
            }

            empty = true;
        }
    }
}
