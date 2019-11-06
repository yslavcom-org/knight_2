using UnityEngine;

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

        private void Start()
        {
            allSlots = slotHolder.transform.childCount;
            slot = new GameObject[allSlots];

            for (int i = 0; i < allSlots; i++)
            {
                slot[i] = slotHolder.transform.GetChild(i).gameObject;

                if(slot[i].GetComponent<Slot>().item == null)
                {
                    slot[i].GetComponent<Slot>().empty = true;
                }
            }
        }

        void Update()
        {
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
            if(other.tag == "Items")
            {
                GameObject itemPickedUp = other.gameObject;
                Item item = itemPickedUp.GetComponent<Item>();
                AddItem(itemPickedUp, item.id, item.amount, item.type, item.description, item.icon);
            }
        }

        void AddItem(GameObject itemObject, int itemId, int itemAmount, string itemType, string itemDescription, Sprite itemIcon)
        {
            for (int i = 0; i < allSlots; i++)
            {
                Slot slot_ = slot[i].GetComponent<Slot>();
                if (slot_.empty)
                {
                    itemObject.GetComponent<Item>().pickedUp = true;

                    slot_.item = itemObject;
                    slot_.id = itemId;
                    slot_.type = itemType;
                    slot_.description = itemDescription;
                    slot_.icon = itemIcon;
                    slot_.amount = itemAmount;

                    itemObject.transform.parent = slot_.transform;
                    itemObject.SetActive(false);

                    slot_.UpdateSlot();
                    slot_.empty = false;
                    return;
                }
            }
        }
    }
}
