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
        EventManager.TriggerEvent(HardcodedValues.evntName__tankShootEventString, KeyCode.Space);

        //Debug.Log("SendShootEvent");
    }
}
