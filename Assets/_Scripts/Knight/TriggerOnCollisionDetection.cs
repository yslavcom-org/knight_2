using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnCollisionDetection : MonoBehaviour
{
    public string _groundTag = "";
    public string event_name = "SphereBlowsUp";

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == _groundTag)
        {
            Debug.Log("Collided");

            EventManager.TriggerEvent(event_name, transform.position);
            this.gameObject.SetActive(false);
        }
    }

}
