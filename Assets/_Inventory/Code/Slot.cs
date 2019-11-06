using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameInventory
{
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        public GameObject item;

        public bool empty;
        public int id;
        public string type;
        public string description;
        public int amount;//this is the quatity of this item

        public Transform slotIconGO;
        public Sprite icon;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            UseItem();
        }

        private void Start()
        {
            slotIconGO = transform.GetChild(0);
        }

        public void UpdateSlot()
        {
            slotIconGO.GetComponent<Image>().sprite = icon;
        }

        public void UseItem()
        {
            item.GetComponent<Item>().ItemUsage();
        }
    }
}
