using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FreeSpaceKeyPressed : MonoBehaviour
{
    public GameObject emitter;
    public GameObject projectile;
    public float projectileForce = 1500f;

    public string event_name = "FreeSpaceKeyPressed";

    private HardcodedValues hardcodedValues;
    private GameObject[] array_of_projectiles;
    private int array_of_projectiles__idx;

    private UnityAction<object> someListener;

    void Awake()
    {
        someListener = new UnityAction<object>(EmitProjectile);

        array_of_projectiles__idx = 0;

        hardcodedValues = FindObjectOfType<HardcodedValues>();
        if (!hardcodedValues)
        {
            Debug.Log("Provide hardcodedValues");
        }
        else
        {
            //instantiate pool of projectiles as an array
            array_of_projectiles = new GameObject[hardcodedValues.projectiles_count__max];
            if (array_of_projectiles.Length > 0)
            {
                for (int i = 0; i < array_of_projectiles.Length; i++)
                {
                    array_of_projectiles[i] = Instantiate(projectile, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    if (null != array_of_projectiles[i])
                    {
                        array_of_projectiles[i].SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("Failed to create the array");
            }
        }
    }

    void OnEnable()
    {
        EventManager.StartListening(event_name, someListener);
    }

    void OnDisable()
    {
        EventManager.StopListening(event_name, someListener);
    }


    void EmitProjectile(object arg)
    {
        Debug.Log("Emit projectile");

        //move a projectile to the right place, set it active and launch
        if (array_of_projectiles__idx >= array_of_projectiles.Length)
        {
            array_of_projectiles__idx = 0;
        }

        var projectile = array_of_projectiles[array_of_projectiles__idx];
        array_of_projectiles__idx++;

        var rigidBody = projectile.GetComponent<Rigidbody>();
        if (rigidBody)
        {
            projectile.SetActive(false);

            projectile.transform.position = emitter.transform.position;
            projectile.transform.rotation = Quaternion.Euler(0, 45, 0);

            projectile.SetActive(true);
            rigidBody.AddForce( emitter.transform.forward * projectileForce);
        }
    }
}
