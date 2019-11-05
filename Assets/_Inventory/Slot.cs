using UnityEngine;
using UnityEngine.UI;

namespace GameInventory
{
    public class Slot : MonoBehaviour
    {
        public GameObject item;

        public bool empty;
        public int id;
        public string type;
        public string description;
        public Sprite icon;

        public void Updateslot()
        {
            this.GetComponent<Image>().sprite = icon;
        }
    }
}
