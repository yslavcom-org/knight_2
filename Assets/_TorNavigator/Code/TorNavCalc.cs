using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorNavCalc : MonoBehaviour
{
    public enum EnNavStat
    {
        Idle,
        Active
    };

    static private float PosNegAngle(Vector3 a1, Vector3 a2, Vector3 normal)
    {
        //Where "normal" is the reference Vector you are determining the clockwise / counter-clockwise rotation around.
        float angle = Vector3.Angle(a1, a2);
        float sign = Mathf.Sign(Vector3.Dot(normal, Vector3.Cross(a1, a2)));
        return angle * sign;
    }

    static private float GetObjectHorizontalAngleInWorld(ref Transform transform)
    {
        float object_angle = PosNegAngle(transform.forward, Vector3.forward, Vector3.up);

        //get the tank_angle to the same basis as the ctrl_angle
        if (object_angle <= 0 && object_angle >= -180)
        {
            object_angle = -1 * object_angle;
        }
        else if (object_angle > 0 && object_angle <= 180)
        {
            object_angle = 360 - object_angle;
        }

        return object_angle;
    }

    static public void ResetStateMachine(ref EnNavStat enNavStat)
    {
        enNavStat = EnNavStat.Idle;
    }

    //keep the direction choosen at the very start: either always ahead or always reverse. 
    //performs U-turns.
    static public void HandleToroidTouchNavigation__KeepDirection(ref Transform transform, 
        ref EnNavStat enNavStat,
        float navigationToroidalAngle,
        int forward,
        out int outForward,
        out int outRotate
        )
    {
        int rotate = 0;

        //angle in the control element
        int ctrl_world_angle = (int)navigationToroidalAngle;

        //get the tank angle
        int tank_world_angle = (int)GetObjectHorizontalAngleInWorld(ref transform);

        int diffr_angle = ctrl_world_angle - tank_world_angle;
        int quarter_ctrl = ctrl_world_angle / 90;
        int quarter_tank = tank_world_angle / 90;

        if (EnNavStat.Idle == enNavStat)
        {
            enNavStat = EnNavStat.Active;
            if (ctrl_world_angle == tank_world_angle)
            {
                forward = 1;
            }
            else
            {
                if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
                {
                    forward = 1;
                }
                else if ((quarter_ctrl == 3 && quarter_tank == 0)
                    || (quarter_ctrl == 0 && quarter_tank == 3))
                {
                    if (Mathf.Abs(diffr_angle) >= 270)
                    {
                        forward = 1;
                    }
                    else
                    {
                        forward = -1;
                    }
                }
                else
                {
                    forward = -1;
                }
            }
        }

        if (ctrl_world_angle == tank_world_angle)
        {
            rotate = 0;
        }
        else if (forward > 0)
        {
            if (Mathf.Abs(ctrl_world_angle - tank_world_angle) <= 90)
            {
                rotate = (ctrl_world_angle > tank_world_angle) ? 1 : -1;
            }
            else if ((quarter_ctrl == 3 && quarter_tank == 0)
                || (quarter_ctrl == 0 && quarter_tank == 3))
            {
                if (Mathf.Abs(diffr_angle) >= 270)
                {
                    rotate = (ctrl_world_angle > tank_world_angle) ? -1 : 1;
                }
                else
                {
                    if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? -1 : 1;
                    else rotate = (diffr_angle < 0) ? 1 : -1;
                }
            }
            else
            {
                if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? -1 : 1;
                else rotate = (diffr_angle < 0) ? 1 : -1;
            }
        }
        else if (forward < 0)
        {
            if (Mathf.Abs(diffr_angle) < 180) rotate = (diffr_angle < 0) ? 1 : -1;
            else rotate = (diffr_angle < 0) ? -1 : 1;
        }

        //Debug.Log(string.Format("tank_angle = {0}, control_angle = {1}, diffr_angle = {2}, rotate = {3}", tank_world_angle, ctrl_world_angle, diffr_angle, rotate));

        outForward = forward;
        outRotate = rotate;
    }
}
