using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAudioListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EnsureSingleAudioListener();
    }


    void EnsureSingleAudioListener()
    {
        //turn off AudioListener component if one already exists in the scene
        //WARNING! This finds disabled listener components!! (although not inactive GOs with listeners on them)
        AudioListener[] myListeners = FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

        int totalListeners = 0;//find out how many listeners are actually active
        foreach (AudioListener thisListener in myListeners)
        {
            if (thisListener.enabled) { totalListeners++; }
        }

        if (totalListeners > 1)
        {
            //turn off my audioListener component
            AudioListener al = GetComponent<AudioListener>();
            al.enabled = false;
        }
        else
        {
            //turn on my audioListener component
            AudioListener al = GetComponent<AudioListener>();
            al.enabled = true;
            //print ("turn on audio "+name);
        }
    }


}
