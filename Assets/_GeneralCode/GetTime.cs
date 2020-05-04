using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTime : MonoBehaviour
{
    static public int TimeSinceStartInt()
    {
        return (int)(Time.time + 0.5f);
    }

    static public float TimeSinceStartFloat()
    {
        return Time.time;
    }
}
