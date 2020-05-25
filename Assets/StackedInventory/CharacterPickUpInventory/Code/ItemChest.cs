using UnityEngine;

namespace Iar.StackedInventory
{
    public class ItemChest : MonoBehaviour
    {
        [SerializeField] StackedItem item;
        [SerializeField] PickUpEquipmentPanel equipmentPanel;

        private void Init()
        {
            if (null == equipmentPanel)
            {
                equipmentPanel = FindObjectOfType<PickUpEquipmentPanel>();
            }
        }

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            Init();
        }

        private void OnValidate()
        {
            Init();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (equipmentPanel.AddItem((EquipableStackedItem)item))
            {
                transform.gameObject.SetActive(false);
            }
        }
    }
}
