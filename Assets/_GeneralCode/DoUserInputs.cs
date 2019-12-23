using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoUserInputs : MonoBehaviour
{
    public static void HandleKeyboard(out int forwardInput, out int rotationInput, out bool navigationKeyPressed)
    {
        //key press
        bool navigate = TouchOrMouseClick.KeyPress(out float outForwardInput, out float outRotationInput);

        if (navigate)
        {
            forwardInput = outForwardInput == 0
                ? 0 : outForwardInput > 0
                ? 1 : -1;
            rotationInput = outRotationInput == 0
                ? 0 : outRotationInput > 0
                ? 1 : -1;
        }
        else
        {
            forwardInput = 0;
            rotationInput = 0;
        }
        navigationKeyPressed = navigate;
    }

    public static void HandleToroidNavigator(ref ToroidNavigator toroidNavigator, out float navigationToroidalAngle, out float navigationToroidalGearNum, out bool navigationToroidalControlActive)
    {
        navigationToroidalAngle = 0f;
        navigationToroidalGearNum = 0f;

        //toroidal navigation control
        bool navigate = false;
        if (toroidNavigator)
        {
            navigate = toroidNavigator.GetPressedDirection(out float navAngle, out int gearNumber);
            if (navigate)
            {
                navigationToroidalAngle = navAngle;
                navigationToroidalGearNum = gearNumber;
            }
        }

        navigationToroidalControlActive = navigate;
    }
}
