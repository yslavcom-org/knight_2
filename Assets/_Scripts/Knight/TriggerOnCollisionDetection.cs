using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if true

public class TriggerOnCollisionDetection : MonoBehaviour
{
    public string _groundTag = "";
    public string event_name = "SphereBlowsUp";

    public LayerMask _groundLayer;

    void FixedUpdate()
    {
        if (IsGrounded())
        {
            Debug.Log("Collided with ground");

            EventManager.TriggerEvent(event_name, transform.position);
            this.gameObject.SetActive(false);
        }
    }



    bool IsGrounded()
    {
        Vector3 position = this.gameObject.transform.position;
        Vector3 direction = Vector3.down;
        float distance = 1.0f;

        var hit = Physics.Raycast(position, direction, distance, _groundLayer);
        
        return hit;
    }
}


#else

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

#endif
