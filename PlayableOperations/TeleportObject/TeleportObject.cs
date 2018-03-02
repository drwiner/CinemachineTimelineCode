using UnityEngine;
using UnityEngine.Playables;

public class TeleportObjectPlayable : PlayableBehaviour
{
    private GameObject _thingToMove;
    private Transform _startTransform;
    private Transform _endTransform;

    public void Initialize(GameObject thingToMove, Transform startTransform, Transform endTransform)
    {
        _thingToMove = thingToMove;
        _startTransform = startTransform;
        _endTransform = endTransform;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info) 
    {
        if (_startTransform != null) 
        {
            MoveObject(_startTransform);
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info) 
    {
        if ( _endTransform != null) 
        {
            MoveObject(_endTransform);
        }
    }

    private void MoveObject(Transform targetTransform)
    {
        _thingToMove.transform.position = targetTransform.position;
        _thingToMove.transform.rotation = targetTransform.rotation;
    }
}