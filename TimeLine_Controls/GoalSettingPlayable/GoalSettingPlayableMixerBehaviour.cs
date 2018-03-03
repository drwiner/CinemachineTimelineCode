using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SteeringNamespace;

public class GoalSettingPlayableMixerBehaviour : PlayableBehaviour
{
    private DynoSteering ds_force;
    private DynoSteering ds_torque;
    private DynoSteering ds;
    private KinematicSteeringOutput kso;

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<GoalSettingPlayableBehaviour> inputPlayable = (ScriptPlayable<GoalSettingPlayableBehaviour>)playable.GetInput(i);
            GoalSettingPlayableBehaviour input = inputPlayable.GetBehaviour ();
            
            // Use the above variables to process each frame of this playable.
            updateSteering(input, (float)playable.GetTime());
        }
    }

    private void updateSteering(GoalSettingPlayableBehaviour input, float time)
    {
       
        ds_force = input.darv.getSteering();
        ds_torque = input.daln.getSteering();

        ds = new DynoSteering();
        ds.force = ds_force.force;
        ds.torque = ds_torque.torque;

        // Update Kinematic Steering
        kso = input.k.updateSteering(ds, time);
        //Debug.Log(kso.position);
        input.agent.transform.position = new Vector3(kso.position.x, input.agent.transform.position.y, kso.position.z);
        input.agent.transform.rotation = Quaternion.Euler(0f, kso.orientation * Mathf.Rad2Deg, 0f);
    }
}
