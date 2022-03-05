using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
    public delegate void EventFunction(object data);

    public enum EventType
    {
        StateDecided,
        CanMakeScreenshot,
        MakeScreenshotWithBottomCamera,
        BottomScreenshotWasMade,
        RecordingStateChanged,
        IntroAnimStarted,
        AudioPlay,
        AudioStop,
        AudioFadeOut,

    }

    static Dictionary<EventType, List<EventFunction>> subscribers = new Dictionary<EventType, List<EventFunction>>();

    public static void Subscribe(EventType type, EventFunction func)
    {
        //LoggingManager.Log("subscribing to " + type.ToString());
        if (!subscribers.ContainsKey(type))
            subscribers[type] = new List<EventFunction>();
        if (!subscribers[type].Contains(func))
            subscribers[type].Add(func);
    }

    public static void Unsubscribe(EventType type, EventFunction func)
    {
        if (!subscribers.ContainsKey(type) || !subscribers[type].Contains(func))
        {
            Debug.LogError("tried to unsubscribe but was not registered: " + type.ToString());
            return;
        }
        subscribers[type].Remove(func);
    }

    public static void Dispatch(EventType type, object data)
    {
        Debug.Log("event dispatched: " + type.ToString());

        if (subscribers.ContainsKey(type))
        {
            foreach (EventFunction func in subscribers[type].ToArray())
            {
                func.Invoke(data);
            }
        }
    }
}