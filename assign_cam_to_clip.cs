using Cinemachine.Timeline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class assign_cam_to_clip : MonoBehaviour {
    public Cinemachine.CinemachineVirtualCamera left_angle;
    public Cinemachine.CinemachineVirtualCamera right_angle;
    //public TimelineAsset timeline;
   // public GameObject main_camera_object;
    // Use this for initialization
    void Start() {
        GameObject main_camera_object = GameObject.Find("Main Camera");
        PlayableDirector director = GetComponent<PlayableDirector>();

        // var timeline = GetComponent<TimelineAsset>();
        //director.Pause();
        //  director.playableAsset.CreatePlayable.

        //Sucess:
        //        PlayableBinding track = director.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
        //        director.SetGenericBinding(track.sourceObject, settable_camera);

        // timeline = (TimelineAsset)Resources.Load("RightAngleShotTimeline");
        // timeline = new TimelineAsset();

        TimelineAsset timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        TrackAsset track = timeline.CreateTrack<CinemachineTrack>(null, "trackname");

        // use this 
       // TrackAsset track = timeline.GetRootTrack(0);

        var clip = track.CreateDefaultClip();
        clip.start = 0.0;
        clip.duration = 1;
        clip.easeOutDuration = 2;
        clip.displayName = "justcreated";

        var cinemachineShot = clip.asset as CinemachineShot;
        cinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, left_angle);

        var clip2 = track.CreateDefaultClip();
        clip2.start = 0.85;
        clip2.duration = 2;
        clip2.easeInDuration = 2;
        var cinemachineShot2 = clip2.asset as CinemachineShot;
        cinemachineShot2.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cinemachineShot2.VirtualCamera.exposedName, right_angle);


        //        var track = timeline.CreateTrack(CinemachineTrack, )//

        //      var track = timeline.CreateTrack<CinemachineTrack>(timeline, "just_made");
        //    director.SetGenericBinding()
        // TrackAsset track = timeline.outputs.First(c => c.streamName == "Cinemachine Track");
        //   var track = timeline.outputs[0].sourceObject;
        //director.playableAsset.CreatePlayable(director.playableGraph, this.gameObject);

        //  track.Set
        //track.
        //TimelineAsset this_timeline = director.GetComponent<TimelineAsset>();
        //CinemachineTrack this_timeline = director.GetComponent<CinemachineTrack>();
        //  director.

        //TimelineAsset this_timeline = this.GetComponent<TimelineAsset>();
        //this_timeline.
        //TrackAsset first_track = this_timeline.GetOutputTrack(0);

        //   CinemachineTrack first_track = this_timeline.GetClips()


        //CinemachineTrack first_track = this.GetComponent<CinemachineTrack>();

        // The binding fields for the first track
        //var binding = director.GetGenericBinding(first_track);

        //foreach (var c in track.GetClips())
        //{
        //    var cinemachineShot = c.asset as CinemachineShot;

        //    // remember the virtual camera (an exposed reference) refers to an object in the scene
        //    cinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        //    // for now, set each clip to have the settable camera.
        //    director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, settable_camera);
        //}
        director.SetGenericBinding(track, main_camera_object);
        
        //director.GetGenericBinding(this);
        //this.GetComponent<TimelineAsset>().
        // var first_param = this.GetComponent<TimelineAsset>().CreateTrack(ControlTrack,)
        director.Play(timeline);
    }

    // Update is called once per frame
    void Update() {

    }
}