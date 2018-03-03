using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class GoalSettingPlayableClip : PlayableAsset, ITimelineClipAsset
{
    public GoalSettingPlayableBehaviour template = new GoalSettingPlayableBehaviour ();
    public ExposedReference<GameObject> agent;
    public ExposedReference<GameObject> goal;


    public ClipCaps clipCaps
    {
        get { return ClipCaps.Extrapolation; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<GoalSettingPlayableBehaviour>.Create (graph, template);
        GoalSettingPlayableBehaviour clone = playable.GetBehaviour ();
        clone.agent = agent.Resolve (graph.GetResolver ());
        clone.goal = goal.Resolve (graph.GetResolver ());
        
        //clone.darv = clone.agent.GetComponent<DynoArrive>();
        //clone.daln= clone.agent.GetComponent<DynoAlign>();
        return playable;
    }
}
