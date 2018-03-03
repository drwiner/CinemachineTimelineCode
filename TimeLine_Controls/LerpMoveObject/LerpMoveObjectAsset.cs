using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LerpMoveObjectAsset : PlayableAsset
{
    public ExposedReference<GameObject> ObjectToMove;
    public ExposedReference<Transform> LerpMoveTo;
    public ExposedReference<Transform> LerpMoveFrom;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<LerpMoveObjectPlayable>.Create(graph);
        var lerpMoveObjectPlayable = playable.GetBehaviour();

        var objectToMove = ObjectToMove.Resolve(playable.GetGraph().GetResolver());
        var lerpMoveTo = LerpMoveTo.Resolve(playable.GetGraph().GetResolver());
        var lerpMoveFrom = LerpMoveFrom.Resolve(playable.GetGraph().GetResolver());

        lerpMoveObjectPlayable.Initialize(objectToMove, lerpMoveFrom, lerpMoveTo);
        return playable;
    }
}
