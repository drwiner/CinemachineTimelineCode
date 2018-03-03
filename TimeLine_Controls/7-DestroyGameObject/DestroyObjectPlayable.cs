using UnityEngine;
using UnityEngine.Playables;

public class DestroyObjectPlayable : PlayableBehaviour
{
    private GameObject _objectToDestroy;

    public void Initialize(GameObject gameObject)
    {
        _objectToDestroy = gameObject;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        GameObject.Destroy(_objectToDestroy);
    }
}
