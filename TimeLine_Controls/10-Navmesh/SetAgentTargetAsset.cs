using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;

public class SetAgentTargetAsset : PlayableAsset
{
    public ExposedReference<NavMeshAgent> Agent;
    public ExposedReference<Transform> Target;

    public float AgentSpeed;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SetAgentTargetPlayable>.Create(graph);
        var agentTargetPlayable = playable.GetBehaviour();

        var agent = Agent.Resolve(graph.GetResolver());
        var target = Target.Resolve(graph.GetResolver());
        
        agentTargetPlayable.Initialize(AgentSpeed, target, agent);
        return playable; 
    }

}