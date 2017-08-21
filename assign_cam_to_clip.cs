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

    // Use this for initialization
    void Start() {
        cam_dict = new Dictionary<string, CinemachineVirtualCamera>();

        foreach(CinemachineVirtualCamera Cam in prim_cams)
        {
            cam_dict.Add(Cam.Name, Cam);
        }

        GameObject main_camera_object = GameObject.Find("Main Camera");
        PlayableDirector director = GetComponent<PlayableDirector>();
        

        

        TimelineAsset timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        TrackAsset track = timeline.CreateTrack<CinemachineTrack>(null, "trackname");


      
        var clip = track.CreateDefaultClip();
        clip.start = 0.0;
        clip.duration = 4;
        clip.displayName = "justcreated";


        // Camera 1
        CinemachineVirtualCamera left_angle = (CinemachineVirtualCamera)prim_cams[0];
        var cinemachineShot = clip.asset as CinemachineShot;
        cinemachineShot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, left_angle);


        // Camera 2
        CinemachineVirtualCamera right_angle = (CinemachineVirtualCamera)prim_cams[1];
        var clip2 = track.CreateDefaultClip();
        clip2.start = 4;
        clip2.duration = 4;
        var cinemachineShot2 = clip2.asset as CinemachineShot;
        cinemachineShot2.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cinemachineShot2.VirtualCamera.exposedName, right_angle);


        // Camera 3
        GameObject overhead = (GameObject) special_shots[0];
        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
        var clip3 = ctrack.CreateDefaultClip();
        clip3.start = 8;
        clip3.duration = 8;
        var controlshot =  clip3.asset as ControlPlayableAsset;
        controlshot.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(controlshot.sourceGameObject.exposedName, overhead);

        // Set cinemachine brain as track's game object
        director.SetGenericBinding(track, main_camera_object);
        
        // Set it to play when ready.
        director.Play(timeline);
    }

    // Update is called once per frame
    void Update() {

    }
}