using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySweepLine : MonoBehaviour
{
    MyRadar.RadarSweep radarSweep;

    public void SetRadarSweep(MyRadar.RadarSweep radarSweep)
    {
        this.radarSweep = radarSweep;
    }

    public void ResetSweepLine()
    {
        this.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (null == radarSweep) return;

        this.transform.Rotate(0f, 0f, radarSweep.RotationAngle);
    }
}
