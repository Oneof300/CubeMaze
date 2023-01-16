using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerListener : MonoBehaviour
{
    [SerializeField] DelayedResponse enter;
    [SerializeField] DelayedResponse exit;

    void OnTriggerEnter(Collider other)
    {
        Invoke("EnterResponse", enter.delay);
        // enter.Response.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        Invoke("ExitResponse", exit.delay);
        // exit.Response.Invoke();
    }

    void EnterResponse() => enter.response.Invoke();
    void ExitResponse() => exit.response.Invoke();
}

[Serializable]
public struct DelayedResponse
{
    public float delay;
    public UnityEvent response;
}
