using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : PoolObject
{
    const int speed = 25;

    TrailRenderer trail;
    float trailTime;

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
        if (trail)
        {
            trailTime = trail.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += Vector3.one * Time.deltaTime * 3;
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public override void OnObjectReuse()
    {
        if (trail)
        {
            trail.time = -1;
            Invoke("ResetTrail", 0.1f);
        }
        transform.localScale = Vector3.one;
    }

    private void ResetTrail()
    {
        trail.time = trailTime;
    }
}
