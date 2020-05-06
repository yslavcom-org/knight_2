using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFireButton : MonoBehaviour
{
    public void ButtonClicked__LaunchMissile()
    {
        SendShootEvent(HardcodedValues.evntArg__tankShootEventString_Missile);
    }

    public void ButtonClicked__ShootGun()
    {
        SendShootEvent(HardcodedValues.evntArg__tankShootEventString_Gun);
    }

    void SendShootEvent(string arg)
    {
        EventManager.TriggerEvent(HardcodedValues.evntName__tankShootEventString, arg);
    }
}
