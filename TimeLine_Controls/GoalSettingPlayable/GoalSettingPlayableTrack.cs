using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.4801038f, 0.6436585f, 0.8161765f)]
[TrackClipType(typeof(GoalSettingPlayableClip))]
public class GoalSettingPlayableTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<GoalSettingPlayableMixerBehaviour>.Create (graph, inputCount);
    }
}
