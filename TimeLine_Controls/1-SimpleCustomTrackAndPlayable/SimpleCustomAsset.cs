using UnityEngine;
using UnityEngine.Playables;

public class SimpleCustomAsset : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<SimpleCustomPlayable>.Create(graph);
    }
}