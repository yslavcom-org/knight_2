using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TerrainExplosionSmoke : MonoBehaviour
{
    public string event_name = "SphereBlowsUp";

    public GameObject WhiteSmoke;
    public const int white_smoke__max = 5;
    private int white_smoke__idx;
    private ObjectWithCountdownTimer[] white_smoke__array;
    private float smoke_duration_time = 10f;

    private UnityAction<object> someListener;

    void Awake()
    {
        someListener = new UnityAction<object>(SphereBlowsUp);

        white_smoke__array = new ObjectWithCountdownTimer[white_smoke__max];
    }

    private void Start()
    {
        for (int i = 0; i < white_smoke__max; i++)
        {
            if (white_smoke__array[i] == null)
            {
                var obj = new ObjectWithCountdownTimer(WhiteSmoke);
                white_smoke__array[i] = obj;
            }
            white_smoke__array[i].Activation(false, new Vector3(0,0,0), smoke_duration_time);
        }
        white_smoke__idx = 0;
    }

    void OnEnable()
    {
        EventManager.StartListening(event_name, someListener);
    }

    void OnDisable()
    {
        EventManager.StopListening(event_name, someListener);
    }

    void SphereBlowsUp(object arg)
    {
        if (++white_smoke__idx >= white_smoke__array.Length)
        {
            white_smoke__idx = 0;
        }

        if (white_smoke__array[white_smoke__idx] == null)
        {
            //Debug.Log("init WhiteSmoke");
        }
        else
        {
            Transform trnsfrm = (Transform)arg;
            white_smoke__array[white_smoke__idx].Activation(true, trnsfrm.position, smoke_duration_time);
        }
    }

    public class ObjectWithCountdownTimer
    {
        float _currentTime = 0f;
        GameObject _gameObject;
        Renderer rend;

        public bool _boActive;

        public ObjectWithCountdownTimer(GameObject gameObject)
        {
            _gameObject = Instantiate(gameObject, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            _gameObject.SetActive(false);

            rend = gameObject.GetComponent<Renderer>();
        }

        public void Activation(bool boActive, Vector3 position, float startingTime)
        {
            _boActive = boActive;
            if (boActive)
            {
                _currentTime = startingTime;
                _gameObject.transform.position = position;
                _gameObject.SetActive(true);
            }
            else
            {
                _gameObject.SetActive(false);
            }
        }
    }
}

