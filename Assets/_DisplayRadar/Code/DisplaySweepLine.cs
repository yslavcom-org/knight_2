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

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(0f, 0f, radarSweep.RotationAngle);
    }
}
