using UnityEngine;
using UnityEngine.Playables;

public class TeleportObject : PlayableAsset
{
    public ExposedReference<GameObject> ThingToMove;
    public ExposedReference<Transform> StartTransform;
    public ExposedReference<Transform> EndTransform;

    private GameObject _thingToMove;
    private Transform _startTransform;
    private Transform _endTransform;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TeleportObjectPlayable>.Create(graph);
        var teleportObjectPlayable = playable.GetBehaviour();

        var thingToMove = ThingToMove.Resolve(graph.GetResolver());
        var startTransform = StartTransform.Resolve(graph.GetResolver());
        var endTransform = EndTransform.Resolve(graph.GetResolver());
        
        teleportObjectPlayable.Initialize(thingToMove, startTransform, endTransform);
        return playable;
    }
}