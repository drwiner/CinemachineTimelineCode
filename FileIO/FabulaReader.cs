using System.IO;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Collections.Generic;

public class FabulaReader : MonoBehaviour {

    public string xml_name;
    private FabulaContainer fc;

    //public Dictionary<string, FabulaAction> navDict = new Dictionary<string, FabulaAction>();

    // tracks
    private TrackAsset ctrack;
    private PlayableTrack ntrack;

    // director and timeline
    private PlayableDirector director;
    private TimelineAsset timeline;

    // timeline clips
    private ControlPlayableAsset controlAnim;
    private TimelineClip control_track_clip;
    private TimelineClip nav_track_clip;
    private TimelineClip nav_track_clip2;

    // game objects
    private GameObject agent;
    private GameObject location;
    private GameObject starting_location;
    private GameObject animTimelineObject;

    // Use this for initialization
    void Awake () {
        fc = FabulaContainer.Load(Path.Combine(Application.dataPath, "Scripts//CinemachineTimelineCode//FileIO//fabula.xml"));

        timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        director = GetComponent<PlayableDirector>();

        ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
        ntrack = timeline.CreateTrack<PlayableTrack>(null, "nav_track");

        foreach (FabulaClip clip in fc.Clips)
        {
            if (clip.Type.Equals("animate")){
                populateAnimation(clip);
            }
            else if (clip.Type.Equals("navigate")){
                populateNavigation(clip);
            } else
            {
                Debug.Log("What TYPE of clip is this?");
            }
        }
        director.Play(timeline);
    }
	
	// Update is called once per frame
	private void populateAnimation(FabulaClip clip) {

        // 3 game objects
        agent = GameObject.Find(clip.gameobject_name);
        location = GameObject.Find(clip.startingPos_string);
        animTimelineObject = GameObject.Find(clip.animation_string);

        // control track
        control_track_clip = ctrack.CreateDefaultClip();
        control_track_clip.start = clip.start + (float)0.12;
        control_track_clip.duration = clip.duration - (float)0.12;
        control_track_clip.displayName = clip.Name;
        controlAnim = control_track_clip.asset as ControlPlayableAsset;
        AnimateBind(controlAnim, animTimelineObject);

        // nav track
        nav_track_clip = ntrack.CreateClip<LerpMoveObjectAsset>();
        nav_track_clip.start = clip.start;
        nav_track_clip.displayName = clip.Name;
        nav_track_clip.duration = 0.11;
        LerpMoveObjectAsset tp_obj = nav_track_clip.asset as LerpMoveObjectAsset;
        TransformBind(tp_obj, agent, agent.transform, makeCustomizedTransform(location.transform.position, clip.orientation).transform);
        //setClipOffset(animTimelineObject, anim_loc, anim_rot);
    }

    private void populateNavigation(FabulaClip clip)
    {

        agent = GameObject.Find(clip.gameobject_name);
        starting_location = GameObject.Find(clip.startingPos_string);
        location = GameObject.Find(clip.endingPos_string);
        animTimelineObject = GameObject.Find(clip.animation_string);

        // get vector3 corresponding to destination - origin
        Vector3 dest_minus_origin = location.transform.position - starting_location.transform.position;
        float orientation = Mathf.Atan2(dest_minus_origin.x, -dest_minus_origin.z) * Mathf.Rad2Deg - 90f;
        //float orientation = Mathf.Atan2(location.transform.position.z, -location.transform.position.x) * Mathf.Rad2Deg;

        nav_track_clip = ntrack.CreateClip<LerpMoveObjectAsset1>();
        nav_track_clip.start = clip.start;
        nav_track_clip.displayName = (string)clip.Name + "_transport";
        nav_track_clip.duration = (double)0.11;
        LerpMoveObjectAsset1 tp_obj = nav_track_clip.asset as LerpMoveObjectAsset1;
        Transform start_transform = makeCustomizedTransform(starting_location.transform.position, orientation).transform;
        TransformBind1(tp_obj, agent, start_transform);

        nav_track_clip2 = ntrack.CreateClip<LerpMoveObjectAsset>();
        nav_track_clip2.start = clip.start + (float)0.12;
        nav_track_clip2.duration = clip.duration - (float)0.12;
        LerpMoveObjectAsset lerp_clip = nav_track_clip2.asset as LerpMoveObjectAsset;
        Transform end_transform = makeCustomizedTransform(location.transform.position, orientation).transform;
        TransformBind(lerp_clip, agent, start_transform, end_transform);

        // control track - animate
        control_track_clip = ctrack.CreateDefaultClip();
        control_track_clip.start = clip.start + (float)0.12;
        control_track_clip.duration = clip.duration - (float)0.12;
        control_track_clip.displayName = clip.Name;
        controlAnim = control_track_clip.asset as ControlPlayableAsset;
        AnimateBind(controlAnim, animTimelineObject);

        //FabulaAction FA = new FabulaAction();
        //FA.start = (float)control_track_clip.start;
        //FA.duration = (float)control_track_clip.duration;
        //FA.NavObj = agent;
        //FA.end_pos = end_transform.position;
        //FA.start_pos = start_transform.position;
        //FA.orientation = orientation;
        //navDict.Add(clip.Name, FA);
    }

    private void TeleportBind(TeleportObject tpObj, GameObject obj_to_move, Transform start_pos, Transform end_pos)
    {
        tpObj.ThingToMove.exposedName = UnityEditor.GUID.Generate().ToString();
        tpObj.StartTransform.exposedName = UnityEditor.GUID.Generate().ToString();
        tpObj.EndTransform.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(tpObj.ThingToMove.exposedName, obj_to_move);
        director.SetReferenceValue(tpObj.StartTransform.exposedName, start_pos);
        director.SetReferenceValue(tpObj.EndTransform.exposedName, end_pos);
    }

    private void TransformBind1(LerpMoveObjectAsset1 tpObj, GameObject obj_to_move, Transform end_pos)
    {
        tpObj.ObjectToMove.exposedName = UnityEditor.GUID.Generate().ToString();
        tpObj.LerpMoveTo.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(tpObj.ObjectToMove.exposedName, obj_to_move);
        director.SetReferenceValue(tpObj.LerpMoveTo.exposedName, end_pos);
    }

    private void TransformBind(LerpMoveObjectAsset tpObj, GameObject obj_to_move, Transform start_pos, Transform end_pos)
    {
        tpObj.ObjectToMove.exposedName = UnityEditor.GUID.Generate().ToString();
        tpObj.LerpMoveTo.exposedName = UnityEditor.GUID.Generate().ToString();
        tpObj.LerpMoveFrom.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(tpObj.ObjectToMove.exposedName, obj_to_move);
        director.SetReferenceValue(tpObj.LerpMoveTo.exposedName, end_pos);
        director.SetReferenceValue(tpObj.LerpMoveFrom.exposedName, start_pos);
    }

    private void AnimateBind(ControlPlayableAsset cpa, GameObject ato)
    {
        cpa.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cpa.sourceGameObject.exposedName, ato);
    }

    // orientation should be in degrees (i.e. oriention *= Mathf.Rad2Deg)
    private GameObject makeCustomizedTransform(Vector3 pos, float orientation)
    {
        GameObject t = new GameObject();
        t.transform.position = pos;
        t.transform.rotation = Quaternion.Euler(0f, orientation, 0f);
        return t;
    }


}
