using UnityEngine;
using UnityEngine.Playables;

public class ToggleComponentAsset : PlayableAsset
{
    public ExposedReference<MonoBehaviour> Component;

    public bool EnabledOnStart;
    public bool EnabledOnEnd;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ToggleComponentPlayable>.Create(graph);
        var toggleComponentPlayable = playable.GetBehaviour();

        var component = Component.Resolve(graph.GetResolver());

        toggleComponentPlayable.Initialize(component, EnabledOnStart, EnabledOnEnd);
        return playable;
    }	
}