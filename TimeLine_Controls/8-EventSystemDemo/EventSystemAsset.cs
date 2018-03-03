using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

public class EventSystemAsset : PlayableAsset
{
    public ExposedReference<EventTrigger> TimelineEventTrigger;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EventSystemPlayable>.Create(graph);
        var eventSystemPlayable = playable.GetBehaviour();

        var eventTrigger = TimelineEventTrigger.Resolve(graph.GetResolver());

        eventSystemPlayable.Initialize(eventTrigger);
        return playable;
    }
}