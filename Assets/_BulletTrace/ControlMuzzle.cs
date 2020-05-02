using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMuzzle : MonoBehaviour
{
    public ParticleSystem particleSystem;

    #region Built-in Methods
    // Start is called before the first frame update
    void Start()
    {
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        //TestParticleSystem();
    }
    #endregion


    #region Built-in Methods
    public void PlayTrails()
    {
        particleSystem.Play();
        StartCoroutine("StopTrails");
    }
  
    IEnumerator StopTrails()
    {
        yield return new WaitForSeconds(0.5f);
        particleSystem.Stop();
    }
    #endregion


    #region Test Methods
    //test
    float trigger_time_in_seconds = 0;
    const float interval = 5;
    void TestParticleSystem()
    {
        //timer
        float time_in_seconds = GetTime.TimeSinceStartFloat();

        if (time_in_seconds >= (trigger_time_in_seconds + interval))
        {
            PlayTrails();
            trigger_time_in_seconds = time_in_seconds;
        }
   }
    #endregion
}
