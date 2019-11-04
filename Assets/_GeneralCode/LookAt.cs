using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LookAt : MonoBehaviour
{
    public Camera cam;

    void LateUpdate()
    {
        if (cam)
        {
            transform.LookAt(cam.transform.position);
        }
    }
}
