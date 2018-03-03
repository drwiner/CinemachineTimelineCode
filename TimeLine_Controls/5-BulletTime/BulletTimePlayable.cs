using UnityEngine;
using UnityEngine.Playables;

public class BulletTimePlayable : PlayableBehaviour
{
    private float _bulletTimeTimeScale;
    private float _originalTimeScale = 1f;

    public void Initialize(float bulletTimeScale)
    {
        _bulletTimeTimeScale = bulletTimeScale;
    }
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) 
    {
        //This checks if we're actually playing. Prevents it running before the clip has started.
        if (playable.GetTime() <= 0)
            return;
        
        Time.timeScale = Mathf.Lerp (_originalTimeScale, _bulletTimeTimeScale, (float)(playable.GetTime() / playable.GetDuration()));
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        _originalTimeScale = Time.timeScale;
    }
}