using UnityEngine;
using UnityEngine.Events;

public class StackedInventoryInput : MonoBehaviour
{
    [SerializeField] GameObject inventoryGameObject;
    private UnityAction<object> someListener;

    void OnEnable()
    {
        EventManager.StartListening(HardcodedValues.evntName__StackedInventoryClicked, someListener);
    }

    void OnDisable()
    {
        EventManager.StopListening(HardcodedValues.evntName__StackedInventoryClicked, someListener);
    }

    void Awake()
    {
        someListener = new UnityAction<object>(ToggleStackedInventoryDisplay);
    }

    private void ToggleStackedInventoryDisplay(object obj)
    {
        inventoryGameObject.SetActive(!inventoryGameObject.activeSelf);
    }
}
