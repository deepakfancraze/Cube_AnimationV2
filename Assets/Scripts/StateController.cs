using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [SerializeField] State state;

    static StateController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        EventManager.Dispatch(EventManager.EventType.StateDecided, state);
    }

    public static State GetState()
    {
        return instance.state;
    }
}

public enum State
{
    Interaction,
    Video,
    Recording,
}
