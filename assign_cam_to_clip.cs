using Cinemachine.Timeline;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Animations;

public class assign_cam_to_clip : MonoBehaviour {
    private Dictionary<string, CinemachineVirtualCamera> cam_dict;

    public List<CinemachineVirtualCamera> prim_cams;
    public List<GameObject> special_shots;

    //public TimelineAsset timeline;
    // public GameObject main_camera_object;
    // Use this for initialization
    void Start() {
        cam_dict = new Dictionary<string, CinemachineVirtualCamera>();

        foreach(CinemachineVirtualCamera Cam in prim_cams)
        {
            cam_dict.Add(Cam.Name, Cam);
        }

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


      //  CinemachineMixer
        
        // use this 
       // TrackAsset track = timeline.GetRootTrack(0);

        var clip = track.CreateDefaultClip();
        clip.start = 0.0;
        clip.duration = 4;
        clip.displayName = "justcreated";


        CinemachineVirtualCamera left_angle = (CinemachineVirtualCamera)prim_cams[0];
        var cinemachineShot = clip.asset as CinemachineShot;
        cinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, left_angle);

        CinemachineVirtualCamera right_angle = (CinemachineVirtualCamera)prim_cams[1];
        var clip2 = track.CreateDefaultClip();
        clip2.start = 4;
        clip2.duration = 4;
        var cinemachineShot2 = clip2.asset as CinemachineShot;
        cinemachineShot2.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cinemachineShot2.VirtualCamera.exposedName, right_angle);

        GameObject overhead = (GameObject) special_shots[0];
        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
        var clip3 = ctrack.CreateDefaultClip();
        clip3.start = 8;
        clip3.duration = 8;
        
        var controlshot =  clip3.asset as ControlPlayableAsset;
        controlshot.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(controlshot.sourceGameObject.exposedName, overhead);

        //AnimationMixerPlayable mixerPlayable = AnimationMixerPlayable.Create(director.playableGraph, 2);
        //  director.playableGraph.Connect(clip, 0, mixerPlayable, 0);
        //director.playableGraph.Connect(clip2, 0, mixerPlayable, 1);
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
        //director.SetGenericBinding(ctrack, )
        //director.GetGenericBinding(this);
        //this.GetComponent<TimelineAsset>().
        // var first_param = this.GetComponent<TimelineAsset>().CreateTrack(ControlTrack,)
        director.Play(timeline);
    }

    // Update is called once per frame
    void Update() {

    }
}