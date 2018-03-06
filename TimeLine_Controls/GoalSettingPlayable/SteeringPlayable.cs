using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SteeringNamespace;

public class SteeringPlayable : PlayableBehaviour
{
    private DynoBehavior_TimelineControl _Controller;
    private GameObject _gameObject;
    private Vector3 _steerTo;
    private Vector3 _steerFrom;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private bool _arrival, _departure;


    public void Initialize(GameObject gameObject, Vector3 lerpMoveFrom, Vector3 lerpMoveTo, bool departing, bool arriving)
    {
        _gameObject = gameObject;
        _steerTo = lerpMoveTo;
        _steerFrom = lerpMoveFrom;
        _Controller = _gameObject.GetComponent<DynoBehavior_TimelineControl>();
        _arrival = arriving;
        _departure = departing;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!_Controller.isDone())
            _Controller.steering = true;

        //Debug.Log(_gameObject.transform.position);
    }


    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (_gameObject != null)
        {
            _originalPosition = _gameObject.transform.position;
            //Debug.Log(_gameObject.transform.position);
            _gameObject.transform.position = _steerFrom;
            //Debug.Log(_gameObject.transform.position);
            _Controller.Steer(_steerFrom, _steerTo, _departure, _arrival);
            _originalRotation = _gameObject.transform.rotation;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_gameObject != null)
        {
            _originalPosition = _gameObject.transform.position;
            _originalRotation = _gameObject.transform.rotation;
        }
    }
}
