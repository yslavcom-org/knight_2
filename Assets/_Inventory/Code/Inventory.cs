using System.Collections.Generic;
using UnityEngine;

//fixed amount of slts determine the mximum number of items subject to pick up which is limiting.
//but I'm ok to start with this approach.
//I'll change it later (maybe even will get rid off the the inventory cells)

//https://www.youtube.com/watch?v=IhmqxXaK9hY

namespace GameInventory
{
    public class Inventory : MonoBehaviour
    {
        public bool inventoryEnabled;
        public GameObject inventoryObj;

        private int allSlots;
        private int enabledSlots;
        private GameObject[] slot;

        public GameObject slotHolder;

        private Dictionary<int, Slot> usedSlots = new Dictionary<int, Slot>();

        readonly int defaultSlotCount = 10;

        private void Start()
        {
            Slot.OnEmptiedItemId += OnEmptiedItemId;

            if (null != slotHolder)
            {
                allSlots = slotHolder.transform.childCount;
                slot = new GameObject[allSlots];

                for (int i = 0; i < allSlots; i++)
                {
                    slot[i] = slotHolder.transform.GetChild(i).gameObject;
                }
            }
            else
            {
                allSlots = defaultSlotCount;
                slot = new GameObject[allSlots];

                for (int i = 0; i < allSlots; i++)
                {
                    slot[i] = new GameObject();
                    slot[i].transform.name = transform.name + "inventory_slot_" + i;
                    slot[i].transform.parent = transform;
                    slot[i].AddComponent<Slot>();
                }
            }
        }

        private void OnDisable()
        {
            Slot.OnEmptiedItemId -= OnEmptiedItemId;
        }

        void Update()
        {
            if (null == inventoryObj) return;
            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryEnabled = !inventoryEnabled;
            }

            if (inventoryEnabled)
            {
                inventoryObj.SetActive(true);
            }
            else
            {
                inventoryObj.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == HardcodedValues.StrPickUpObjectTag)
            {
                GameObject itemPickedUp = other.gameObject;
                Item item = itemPickedUp.GetComponent<Item>();
                AddItem(itemPickedUp, item.id, item.amount, item.type, item.description, item.icon);
            }
        }

        void AddItem(GameObject itemObject, int itemId, int itemAmount, string itemType, string itemDescription, Sprite itemIcon)
        {
            if (usedSlots.ContainsKey(itemId) != false)
            {
                //add amount
                Slot slot_ = usedSlots[itemId];
                slot_.amount += itemAmount;
                slot_.UpdateSlotBusy();

                itemObject.SetActive(false);
                return;
            }
            else
            {
                for (int i = 0; i < allSlots; i++)
                {
                    Slot slot_ = slot[i].GetComponent<Slot>();
                    if (!slot_.IfSlotBusy())
                    {
                        itemObject.GetComponent<Item>().pickedUp = true;

                        slot_.item = itemObject;
                        slot_.id = itemId;
                        slot_.type = itemType;
                        slot_.description = itemDescription;
                        slot_.icon = itemIcon;
                        slot_.amount = itemAmount;

                        slot_.UpdateSlotBusy();
                        usedSlots.Add(itemId, slot_);

                        itemObject.SetActive(false);
                        return;
                    }
                }
            }
        }

        void OnEmptiedItemId(int parentId, int itemId)
        {
            if (usedSlots.ContainsKey(itemId) != false)
            {
                usedSlots.Remove(itemId);
            }
        }

        public int RequestItemsDispatch(int itemId, int itemRequestAmount)
        {
            int dispatchAmount = 0;

            if (usedSlots == null) return dispatchAmount;

            if (usedSlots.ContainsKey(itemId) == false) return dispatchAmount;

            var slot_ = usedSlots[itemId];

            dispatchAmount = slot_.UseItem(itemRequestAmount);

            return dispatchAmount;
        }
    }
}
