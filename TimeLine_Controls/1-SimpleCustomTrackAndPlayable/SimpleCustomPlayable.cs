using UnityEngine;
using UnityEngine.Playables;

public class SimpleCustomPlayable : PlayableBehaviour
{
    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("Graph start");
    }

    public override void OnGraphStop(Playable playable)
    {
        Debug.Log("Graph stop");
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info){
        Debug.Log("Play State Playing");
    }

    public override void OnBehaviourPause(Playable playable, FrameData info){
        Debug.Log("Play State Paused");
    }
}
