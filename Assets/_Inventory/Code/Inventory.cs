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
        private GameObject[] arrayOfSlots;

        public GameObject slotHolder;

        private Dictionary<int, Slot> usedSlots = new Dictionary<int, Slot>();

        readonly int defaultSlotCount = 10;

        private void Start()
        {
            Slot.OnEmptiedItemId += OnEmptiedItemId;

            CreateArrayOfSlots();
        }

        public void SetId(int id)
        {
            CreateArrayOfSlots();

            //var inventoryMenuId = GetComponent<MyTankGame.IObjectId>(); // it's inventory displayed as menu (Inventory in the scene) and is linked to the main player
            //if(null != inventoryMenuId)
            {
                foreach(var el in arrayOfSlots)
                {
                    if(null != el)
                    {
                        Slot slot_ = el.GetComponent<Slot>();
                        slot_.SetId(id);
                    }
                }
               // inventoryMenuId.SetId(id);
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
                    Slot slot_ = arrayOfSlots[i].GetComponent<Slot>();
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

        void CreateArrayOfSlots()
        {
            if (null != arrayOfSlots)
            {
                //already created
                return;
            }

            if (null != slotHolder)
            {
                allSlots = slotHolder.transform.childCount;
                arrayOfSlots = new GameObject[allSlots];

                for (int i = 0; i < allSlots; i++)
                {
                    arrayOfSlots[i] = slotHolder.transform.GetChild(i).gameObject;
                }
            }
            else
            {
                allSlots = defaultSlotCount;
                arrayOfSlots = new GameObject[allSlots];

                for (int i = 0; i < allSlots; i++)
                {
                    arrayOfSlots[i] = new GameObject();
                    arrayOfSlots[i].transform.name = transform.name + "inventory_slot_" + i;
                    arrayOfSlots[i].transform.parent = transform;
                    arrayOfSlots[i].AddComponent<Slot>();
                }
            }
        }
    }
}
