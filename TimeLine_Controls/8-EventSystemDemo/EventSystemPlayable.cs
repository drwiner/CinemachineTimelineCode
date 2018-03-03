using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

public class EventSystemPlayable : PlayableBehaviour
{
    private EventTrigger _eventTrigger;

    private bool _alreadyTriggered = false;

    public void Initialize(EventTrigger eventTrigger)
    {
        _eventTrigger = eventTrigger;
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info) 
    {
        if (!_alreadyTriggered && _eventTrigger != null) 
        {
            _eventTrigger.OnSubmit (null);
            _alreadyTriggered = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        _alreadyTriggered = false;
    }
}
