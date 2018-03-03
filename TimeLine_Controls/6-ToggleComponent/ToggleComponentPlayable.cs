using UnityEngine;
using UnityEngine.Playables;

public class ToggleComponentPlayable : PlayableBehaviour
{
    private MonoBehaviour _component;
    private bool _enabledOnStart;
    private bool _enabledOnEnd;

    public void Initialize(MonoBehaviour component, bool enabledOnStart, bool enabledOnEnd)
    {
        _component = component;
        _enabledOnStart = enabledOnStart;
        _enabledOnEnd = enabledOnEnd;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (_component != null) 
        {
            _component.enabled = _enabledOnStart;
        }
    }
    
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_component != null) 
        {
            _component.enabled = _enabledOnEnd;
        }	
    }
}