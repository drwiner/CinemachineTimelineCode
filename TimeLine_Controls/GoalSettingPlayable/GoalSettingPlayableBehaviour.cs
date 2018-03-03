using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class GoalSettingPlayableBehaviour : PlayableBehaviour
{
    public GameObject agent;
    public GameObject goal;
    public DynoArrive darv;
    public DynoAlign daln;
    public Kinematic k;


    //public Kinematic k;
    //public DynoArrive darv;
    //public DynoAlign daln;

    public override void OnGraphStart (Playable playable)
    {
        Goal g = agent.GetComponent<Goal>();
        g.setGoal(goal);

        k = agent.GetComponent<Kinematic>();
        darv = agent.GetComponent<DynoArrive>();
        daln = agent.GetComponent<DynoAlign>();
    }
}
