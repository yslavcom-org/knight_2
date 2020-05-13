using System.Collections.Generic;
using UnityEngine;

public class StackedInventory : MonoBehaviour
{
    [SerializeField] List<StackedItem> items;
    [SerializeField] Transform itemsParent;
    [SerializeField] StackedItemSlots[] itemsSlots;

    private void OnValidate()
    {
        if(itemsParent !=null)
        {
            itemsSlots = itemsParent.GetComponentsInChildren<StackedItemSlots>();
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemsSlots.Length; i++)
        {
            itemsSlots[i].Item = items[i];
        }

        for (; i < itemsSlots.Length; i++)
        {
            itemsSlots[i].Item = null;
        }
    }
}
