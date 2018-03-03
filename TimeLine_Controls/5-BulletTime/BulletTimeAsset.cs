using UnityEngine;
using UnityEngine.Playables;

public class BulletTimeAsset : PlayableAsset
{
    public float BulletTimeTimeScale;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<BulletTimePlayable>.Create(graph);
        var bulletTimePlayable = playable.GetBehaviour();

        bulletTimePlayable.Initialize(BulletTimeTimeScale);
        return playable;
    }
}