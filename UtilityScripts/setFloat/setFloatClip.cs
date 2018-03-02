using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class setFloatClip : PlayableAsset
{
    //public setFloatBehaviour template = new setFloatBehaviour ();
    public ExposedReference<GameObject> target_cam;
    public ExposedReference<setFloatData> data;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.ClipIn; }
    }


    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<setFloatBehaviour>.Create (graph);


        var new_val_data = data.Resolve(graph.GetResolver());
        var target_cam_obj = target_cam.Resolve(graph.GetResolver());
        Debug.Log("initialize");
        playable.GetBehaviour().Initialize(new_val_data.new_value, target_cam_obj);

        //var target_cam = target_cam.Resolve(graph.GetResolver());

        //setFloatBehaviour clone = playable.GetBehaviour ();
        //clone.target_cam = target_cam.Resolve (graph.GetResolver ());


        //var setFloatPlayable = playable.GetBehaviour().Initialize(data);

        //setFloatPlayable.Initialize(setFloatPlayable.new_time);
            //.Initialize(template.new_time);
        return playable;
    }
}
