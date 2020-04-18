using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnFreeSpaceKeyPressed : MonoBehaviour
{
    public string str_fire_button_pressed = HardcodedValues.evntName__tankShootEventString;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(event_name);
            EventManager.TriggerEvent(str_fire_button_pressed, KeyCode.Space);
        }
    }
}
