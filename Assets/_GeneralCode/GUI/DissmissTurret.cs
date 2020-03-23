using UnityEngine;

public class DissmissTurret : MonoBehaviour
{
    public void ButtonClicked()
    {
        EventManager.TriggerEvent(HardcodedValues.evntName__dismissTurret, null);
    }
}
