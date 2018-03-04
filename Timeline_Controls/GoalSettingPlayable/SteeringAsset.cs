using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GoalSettingPlayableAsset : PlayableAsset
{
    ////public GoalSettingPlayableBehaviour template = new GoalSettingPlayableBehaviour ();
    //public ExposedReference<GameObject> agent;
    //public ExposedReference<GameObject> goal;


    //public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    //{
    //    var playable = ScriptPlayable<GoalSettingPlayableBehaviour>.Create (graph);
    //    GoalSettingPlayableBehaviour clone = playable.GetBehaviour ();
    //    clone.agent = agent.Resolve (graph.GetResolver ());
    //    clone.goal = goal.Resolve (graph.GetResolver ());

    //    //clone.darv = clone.agent.GetComponent<DynoArrive>();
    //    //clone.daln= clone.agent.GetComponent<DynoAlign>();
    //    return playable;
    //}

    public ExposedReference<GameObject> ObjectToMove;
    public ExposedReference<Transform> SteerTo;
    public ExposedReference<Transform> SteerFrom;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<LerpMoveObjectPlayable>.Create(graph);
        var lerpMoveObjectPlayable = playable.GetBehaviour();

        var objectToMove = ObjectToMove.Resolve(playable.GetGraph().GetResolver());
        var steerTo = SteerTo.Resolve(playable.GetGraph().GetResolver());
        var steerFrom = SteerFrom.Resolve(playable.GetGraph().GetResolver());

        lerpMoveObjectPlayable.Initialize(objectToMove, steerFrom, steerTo);
        return playable;
    }
}
