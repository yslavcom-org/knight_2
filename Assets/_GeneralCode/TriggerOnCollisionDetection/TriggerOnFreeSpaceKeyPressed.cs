using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnFreeSpaceKeyPressed : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(event_name);
            EventManager.TriggerEvent(HardcodedValues.evntName__tankShootEventString, KeyCode.Space);
        }
    }
}
