using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private readonly List<GameEventDelegate> eventListeners = new List<GameEventDelegate>();

    public void Invoke()
    {
        Debug.Log(name + " has been raised.");
        for(int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i](this);
    }

    public void AddListener(GameEventDelegate listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void RemoveListener(GameEventDelegate listener)
    {
        eventListeners.Remove(listener);
    }
}

public delegate void GameEventDelegate(GameEvent gameEvent);