using UnityEngine;
using UnityEngine.Playables;

public class DestroyObjectAsset : PlayableAsset
{
    public ExposedReference<GameObject> ObjectToDestroy;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DestroyObjectPlayable>.Create(graph);
        var destroyObjectPlayable = playable.GetBehaviour();

        var gameObject = ObjectToDestroy.Resolve(graph.GetResolver());
        
        destroyObjectPlayable.Initialize(gameObject);
        return playable;
    }
}