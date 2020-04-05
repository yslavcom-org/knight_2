using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRadarList : MonoBehaviour
{
    List<string> listBlockRadar; // objects marked witht these tags block radar

    // Start is called before the first frame update
    void Start()
    {
        listBlockRadar = new List<string>();
        listBlockRadar.Add(HardcodedValues.StrTag__Building);
        listBlockRadar.Add(HardcodedValues.StrTag__Ground);
        listBlockRadar.Add(HardcodedValues.StrTag__StreetPole);
        listBlockRadar.Add(HardcodedValues.StrTag__Tree);
    }

    public ref List<string> GetList()
    {
        return ref listBlockRadar;
    }
}
