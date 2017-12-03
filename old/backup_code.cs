using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backup_code : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // var timeline = GetComponent<TimelineAsset>();
        //director.Pause();
        //  director.playableAsset.CreatePlayable.

        //Sucess:
        //        PlayableBinding track = director.playableAsset.outputs.First(c => c.streamName == "Cinemachine Track");
        //        director.SetGenericBinding(track.sourceObject, settable_camera);

        // timeline = (TimelineAsset)Resources.Load("RightAngleShotTimeline");
        // timeline = new TimelineAsset();

        //  CinemachineMixer

        // use this 
        // TrackAsset track = timeline.GetRootTrack(0);


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
    }

    // Update is called once per frame
    void Update () {
		
	}
}
