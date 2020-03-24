using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFireButton : MonoBehaviour
{
    public void WhenFireButtonClicked()
    {
        SendShootEvent();
    }

    void SendShootEvent()
    {
        string event_name = HardcodedValues.evntName__tankShootEventString;
        EventManager.TriggerEvent(event_name, KeyCode.Space);

        //Debug.Log("SendShootEvent");
    }
}
