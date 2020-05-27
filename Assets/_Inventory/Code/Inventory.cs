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

        [SerializeField]
        private Dictionary<Iar.StackedInventory.EquipmentType, ItemStorage> usedSlots = new Dictionary<Iar.StackedInventory.EquipmentType, ItemStorage>();

        readonly int defaultSlotCount = 10;

        private void Start()
        {
            ItemStorage.OnEmptiedItemId += OnEmptiedItemId;

            CreateArrayOfSlots();
        }

        public void SetId(int id)
        {
            CreateArrayOfSlots();

            foreach (var el in arrayOfSlots)
            {
                if (null != el)
                {
                    ItemStorage slot_ = el.GetComponent<ItemStorage>();
                    slot_.SetId(id);
                }
            }
        }

        private void OnDisable()
        {
            ItemStorage.OnEmptiedItemId -= OnEmptiedItemId;
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
                AddItemToInventoryManually(other.gameObject);
            }
        }

        public bool AddItemToInventoryManually(GameObject itemPickedUp)
        {
            if (null == itemPickedUp) return false;

            Item item = itemPickedUp.GetComponent<Item>();
            if (null == item) return false;

            return AddItem(itemPickedUp, item.EquipmentType, item.amount);
        }

        bool AddItem(GameObject itemObject, Iar.StackedInventory.EquipmentType EquipmentType, int itemAmount)
        {
            if (usedSlots.ContainsKey(EquipmentType) != false)
            {
                //add amount
                ItemStorage slot_ = usedSlots[EquipmentType];
                slot_.amount += itemAmount;
                slot_.UpdateSlotBusy();

                itemObject.SetActive(false);
                return true;
            }
            else
            {
                for (int i = 0; i < allSlots; i++)
                {
                    ItemStorage slot_ = arrayOfSlots[i].GetComponent<ItemStorage>();
                    if (!slot_.IfSlotBusy())
                    {
                        slot_.item = itemObject;
                        slot_.EquipmentType = EquipmentType;
                        slot_.amount = itemAmount;

                        slot_.UpdateSlotBusy();
                        usedSlots.Add(EquipmentType, slot_);

                        itemObject.SetActive(false);
                        return true;
                    }
                }
            }

            return false;
        }

        int GetThisPlayerId()
        {
            var objectId = GetComponentInParent<MyTankGame.IObjectId>();
            int playerId = objectId.GetId();

            return playerId;
        }

        void OnEmptiedItemId(int parentId, Iar.StackedInventory.EquipmentType EquipmentType)
        {
            int this_playerId = GetThisPlayerId();
            if (this_playerId != parentId) return;

            if (usedSlots.ContainsKey(EquipmentType) != false)
            {
                usedSlots.Remove(EquipmentType);
            }
        }

        public int RequestItemsDispatch(Iar.StackedInventory.EquipmentType EquipmentType, int itemRequestAmount)
        {
            int dispatchAmount = 0;

            if (usedSlots == null)
            {
                return dispatchAmount;
            }

            if (usedSlots.ContainsKey(EquipmentType) == false) return dispatchAmount;

            var slot_ = usedSlots[EquipmentType];

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
                    arrayOfSlots[i].AddComponent<ItemStorage>();
                }
            }
        }
    }
}
