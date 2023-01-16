using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.AddListener(OnEventRaised);
    }

    private void OnDisable()
    {
        Event.RemoveListener(OnEventRaised);
    }

    public void OnEventRaised(GameEvent gameEvent)
    {
        Response.Invoke();
    }
}
