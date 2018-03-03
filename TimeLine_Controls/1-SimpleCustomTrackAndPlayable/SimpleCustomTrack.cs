using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0f, 0f)]
[TrackClipType(typeof(SimpleCustomAsset))]
public class SimpleCustomTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        //return graph.CreateScriptMixerPlayable<SimpleCustomPlayable>(inputCount);
        return ScriptPlayable<SimpleCustomPlayable>.Create(graph, inputCount);
    }
}
