using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;

public class SetAgentTargetPlayable : PlayableBehaviour
{
    private float _agentSpeed;
    private Transform _target;
    public NavMeshAgent _agent;

    public void Initialize(float agentSpeed, Transform target, NavMeshAgent agent)
    {
        _agentSpeed = agentSpeed;
        _target = target;
        _agent = agent;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (_target != null && _agent != null)
        {
            _agent.destination = _target.position;
            _agent.speed = _agentSpeed;
        }
    }
}
