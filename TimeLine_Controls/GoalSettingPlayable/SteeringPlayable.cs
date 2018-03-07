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
    private int _whichList;
    private bool _arrival, _departure, _isMaster;
    private ulong initialFrameId;
    private int accumulated;

    public void Initialize(GameObject gameObject, Vector3 lerpMoveFrom, Vector3 lerpMoveTo, bool departing, bool arriving, bool isMaster)
    {
        _gameObject = gameObject;
        _steerTo = lerpMoveTo;
        _steerFrom = lerpMoveFrom;
        _Controller = _gameObject.GetComponent<DynoBehavior_TimelineControl>();
        _arrival = arriving;
        _departure = departing;
        _isMaster = isMaster;
        _whichList = _Controller.Register(this, _isMaster);
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!_Controller.isDone())
            _Controller.steering = true;

        if (_isMaster && !_Controller.CheckMasterIsPlaying(_whichList))
        {
            if (Mathf.Abs(info.frameId - initialFrameId) < 5)
            {
                Debug.Log("master is playing");
                _Controller.InformMasterIsPlaying(_whichList, true);
                //accumulated++;
                //if (accumulated > 5)
                //{
                //    Debug.Log("master is playing");
                //    _Controller.InformMasterIsPlaying(_whichList, true);
                //}
                
            }
            
        }
        
        //Debug.Log(_gameObject.transform.position);
    }


    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (_gameObject != null)
        {
            
            if (_isMaster || !_Controller.CheckMasterIsPlaying(_whichList))
            {
                _originalPosition = _gameObject.transform.position;
                _originalRotation = _gameObject.transform.rotation;
                _gameObject.transform.position = _steerFrom;
                _Controller.Steer(_steerFrom, _steerTo, _departure, _arrival);
                initialFrameId = info.frameId;
                
                accumulated = 0;
            }
            else
            {
                Debug.Log("child will not play");
            }
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_isMaster)
        {
            Debug.Log("master not playing");
            _Controller.InformMasterIsPlaying(_whichList, false);
        }

        if (_gameObject != null)
        {
            _originalPosition = _gameObject.transform.position;
            _originalRotation = _gameObject.transform.rotation;
        }
    }
}
