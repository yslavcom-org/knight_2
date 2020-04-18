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
        string str_fire_button_pressed = HardcodedValues.evntName__tankShootEventString;
        EventManager.TriggerEvent(str_fire_button_pressed, KeyCode.Space);

        //Debug.Log("SendShootEvent");
    }
}
