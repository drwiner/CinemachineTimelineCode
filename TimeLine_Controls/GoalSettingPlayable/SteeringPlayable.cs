using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SteeringNamespace;

public class GoalSettingPlayable : PlayableBehaviour
{
    //private DynoSteering ds_force;
    //private DynoSteering ds_torque;
    //private DynoSteering ds;
    //private KinematicSteeringOutput kso;

    //// NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    //public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    //{
    //    int inputCount = playable.GetInputCount ();

    //    for (int i = 0; i < inputCount; i++)
    //    {
    //        float inputWeight = playable.GetInputWeight(i);
    //        ScriptPlayable<GoalSettingPlayableBehaviour> inputPlayable = (ScriptPlayable<GoalSettingPlayableBehaviour>)playable.GetInput(i);
    //        GoalSettingPlayableBehaviour input = inputPlayable.GetBehaviour ();

    //        // Use the above variables to process each frame of this playable.
    //        updateSteering(input, (float)playable.GetTime());
    //    }
    //}

    //private void updateSteering(GoalSettingPlayableBehaviour input, float time)
    //{

    //    ds_force = input.darv.getSteering();
    //    ds_torque = input.daln.getSteering();

    //    ds = new DynoSteering();
    //    ds.force = ds_force.force;
    //    ds.torque = ds_torque.torque;

    //    // Update Kinematic Steering
    //    kso = input.k.updateSteering(ds, time);
    //    //Debug.Log(kso.position);
    //    input.agent.transform.position = new Vector3(kso.position.x, input.agent.transform.position.y, kso.position.z);
    //    input.agent.transform.rotation = Quaternion.Euler(0f, kso.orientation * Mathf.Rad2Deg, 0f);
    //}

    private GameObject _gameObject;
    private Transform _steerTo;
    private Transform _steerFrom;

    Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Vector3 _originalScale;

    public void Initialize(GameObject gameObject, Transform lerpMoveFrom, Transform lerpMoveTo)
    {
        _gameObject = gameObject;
        _steerTo = lerpMoveTo;
        _steerFrom = lerpMoveFrom;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (playable.GetTime() <= 0 || _steerTo == null || _gameObject == null || _steerFrom == null)
            return;

        //Debug.Log("original_position: " + _originalPosition);
        //Debug.Log("destination_position: " + _lerpMoveTo.position);
        //Debug.Log("Playable time: " + playable.GetTime());
        //Debug.Log("Playable duration: " + playable.GetDuration());
        //Debug.Log("\n");

        _gameObject.transform.position = Vector3.Lerp(_steerFrom.position, _steerTo.position, (float)(playable.GetTime() / playable.GetDuration()));
        _gameObject.transform.rotation = Quaternion.Lerp(_steerFrom.rotation, _steerTo.rotation, (float)(playable.GetTime() / playable.GetDuration()));
        _gameObject.transform.localScale = Vector3.Lerp(_steerFrom.localScale, _steerTo.localScale, (float)(playable.GetTime() / playable.GetDuration()));
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (_gameObject != null)
        {
            _originalPosition = _gameObject.transform.position;
            _originalRotation = _gameObject.transform.rotation;
            _originalScale = _gameObject.transform.localScale;
        }
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_gameObject != null)
        {
            _originalPosition = _gameObject.transform.position;
            _originalRotation = _gameObject.transform.rotation;
            _originalScale = _gameObject.transform.localScale;
        }
    }
}
