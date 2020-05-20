using UnityEngine;

public class StackedInventoryClicked : MonoBehaviour
{
    public void OnClicked()
    {
        EventManager.TriggerEvent(HardcodedValues.evntName__StackedInventoryClicked, null);
    }
}
