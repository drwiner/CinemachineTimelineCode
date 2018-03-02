using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class setFloatBehaviour : PlayableBehaviour
{
    public GameObject target_cam;
    public float _new_time;

    public override void OnGraphStart (Playable playable)
    {
        //Debug.Log("ongraphstart");
    }

    public void Initialize(float new_time, GameObject tcam)
    {
        target_cam = tcam;
        _new_time = new_time;
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        //Debug.Log("yes");
        //target_cam.GetComponent<timeStorage>().fab_time = _new_time;
        //Debug.Log(target_cam.GetComponentInChildren<set_fabula_timeline>().enabled);
        //target_cam.GetComponentInChildren<set_fabula_timeline>().fab_time = _new_time;
        //Do something with scale;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        target_cam.GetComponent<timeStorage>().fab_time = _new_time;
        Debug.Log(_new_time);
    }
}
