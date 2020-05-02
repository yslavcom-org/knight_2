using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMuzzle : MonoBehaviour
{
    public ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem.Stop();
    }

    // Update is called once per frame
    float timer = 0.0f;
    void Update()
    {
        timer += Time.deltaTime;
        int seconds = (int)(timer % 60);
        bool start_trigger = (0 == (seconds % 5))
            && (0 != seconds);

        bool stop_trigger = (0 == (seconds % 6))
            && (0 != seconds);

        if (start_trigger)
        {
            particleSystem.Play();
        }
        else if (stop_trigger)
        {
            particleSystem.Stop();
        }
    }
}
