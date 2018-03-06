using SteeringNamespace;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



public class DroneSteeringAsset : PlayableAsset
{

    public ExposedReference<GameObject> Boid;
    public bool arrive, depart;
    public Vector3 startPos, endPos;
    //public ExposedReference<SteerClipParams> SteerParams;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DroneSteeringPlayable>.Create(graph);
        var dronePlayable = playable.GetBehaviour();

        //var objectToMove = ObjectToMove.Resolve(playable.GetGraph().GetResolver());
        var boid = Boid.Resolve(playable.GetGraph().GetResolver());
        //var steerTo = SteerTo.Resolve(playable.GetGraph().GetResolver());
        //var steerFrom = SteerFrom.Resolve(playable.GetGraph().GetResolver());
        //var steerParams = SteerParams.Resolve(playable.GetGraph().GetResolver());

        //dronePlayable.Initialize(boid, steerParams.startPos, steerParams.endPos, steerParams.depart, steerParams.arrive);
        dronePlayable.Initialize(boid, startPos, endPos, depart, arrive);
        return playable;
    }
}
