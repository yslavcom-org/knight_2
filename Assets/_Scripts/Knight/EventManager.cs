
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class ThisEvent : UnityEvent<object> { }


// This class holds the messages, you need to attatch it to one gameobject in the scene
public class EventManager : MonoBehaviour
{
    private Dictionary<string, ThisEvent> eventDictionary;

    private static EventManager eventManger;

    // grabs the instance of the event manager
    public static EventManager Instance
    {
        get
        {
            if (!eventManger)
            {
                eventManger = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManger)
                {
                    Debug.Log("There needs to be one active EventManager script on a gameobject in your scene.");
                }
                else
                {
                    eventManger.Init();

                }
            }
            return eventManger;
        }
    }

    // creates dictionary for the events
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, ThisEvent>();
        }
    }

    // function called to insert an event in the dictionary
    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        ThisEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new ThisEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    // removes an event from the dictionary
    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        if (eventManger == null) return;
        ThisEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    // event trigger with an object passed as a parameter.
    public static void TriggerEvent(string eventName, object variant)
    {
        ThisEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(variant);
        }
    }
}
