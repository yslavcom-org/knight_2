using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnFreeSpaceKeyPressed : MonoBehaviour
{
    public string event_name = HardcodedValues.evntName__tankShootEventString;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(event_name);
            EventManager.TriggerEvent(event_name, KeyCode.Space);
        }
    }
}
