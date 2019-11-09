using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameInventory
{
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        public static event Action<int, int> OnPickedItemId = delegate { }; // notify the newly picked item id
        public static event Action<int> OnEmptiedItemId = delegate { }; // notify the item id which became emptied

        public GameObject item;

        private bool empty=true;
        public int id;
        public string type;
        public string description;
        public int amount=0;//this is the quantity of this item

        public Transform slotIconGO;
        public Sprite icon;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (!empty)
            {
                //display data about this item
            }
        }

        private void Start()
        {
            slotIconGO = transform.GetChild(0);
        }

        public void UpdateSlotBusy()
        {
            slotIconGO.GetComponent<Image>().sprite = icon;
            empty = false;

            OnPickedItemId(id, amount);
        }

        public bool IfSlotBusy()
        {
            return !empty;
        }

        public void UseItem(int amount)
        {
            if (!empty)
            {
                item.GetComponent<Item>().ItemUsage();
                if(this.amount <= amount)
                {
                    EmptyItem();
                }
                else
                {
                    this.amount -= amount;
                }
            }
        }

        private void EmptyItem()
        {
            OnEmptiedItemId(id);

            amount = 0;
            slotIconGO.GetComponent<Image>().sprite = null;

            empty = true;
        }
    }
}
