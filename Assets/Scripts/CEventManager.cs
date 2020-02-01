using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CMessagePayload
{
    // NOTE: THIS NEEDS TO BE UPDATED TO INCLUDE ANY PAYLOAD WE MAY NEED TO SEND WITH A MESSAGE 
    // Remember to check for "null" values if using any of these, since we should only include what is required in the payload.
    // Yes, this is suboptimal, but is useful for newer coders. 
    public GameObject source; // This is who sent the message, if needed
    public GameObject target; // This is who the message is "aimed at", even if that is not the unit that received it (Remember: receiver == this )
    public int count; // How many -- useful for counting number of enemy deaths
    public string type; // What type -- useful for counting number of enemy deaths
}

[System.Serializable]
public class PayloadUnityEvent : UnityEvent<CMessagePayload>
{
}

[System.Serializable]
public class CEventManager<TKey>
{
    // Private Variables
    private Dictionary<TKey, PayloadUnityEvent> events;

    public CEventManager()
    {
        events = new Dictionary<TKey, PayloadUnityEvent>();
    }

    public void TriggerEvent(TKey eventID, CMessagePayload message = null)
    {
        if (events.ContainsKey(eventID) && events[eventID] != null) {
            events[eventID].Invoke(message);
        }
    }

    public void RegisterEvent(TKey eventID, UnityAction<CMessagePayload> call)
    {
        if (!events.ContainsKey(eventID) || events[eventID] == null) {
            events[eventID] = new PayloadUnityEvent();
        }

        events[eventID].AddListener(call);
    }


    public void UnRegisterEvent(TKey eventID, UnityAction<CMessagePayload> call)
    {
        if (events[eventID] != null) {
            events[eventID].RemoveListener(call);
        }
    }
}
