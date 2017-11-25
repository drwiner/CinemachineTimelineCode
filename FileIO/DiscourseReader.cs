using System.IO;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine.Timeline;
using Cinemachine;

public class DiscourseReader : MonoBehaviour
{

    public string xml_name;
    public float HEIGHT = 1.5f;
    private DiscourseContainer dc;
    public GameObject fabulaTimeline;

    // tracks
    private TrackAsset ctrack;
    private PlayableTrack ntrack;
    private CinemachineTrack ftrack;
    private PlayableTrack ptrack;

    // director and timeline
    private PlayableDirector director;
    private PlayableDirector fdirector;
    private TimelineAsset timeline;

    // timeline clips
    private ControlPlayableAsset controlAnim;
    private TimelineClip control_track_clip;
    private TimelineClip nav_track_clip;
    private TimelineClip film_track_clip;
    private TimelineClip time_track_clip;

    // game objects
    private GameObject agent;
    private GameObject anchor;
    private GameObject starting_location;
    private GameObject animTimelineObject;

    // vcams
    private GameObject main_camera_object;
    private CinemachineVirtualCamera new_cam;


    void Start()
    {
        fdirector = fabulaTimeline.GetComponent<PlayableDirector>();
        fdirector.initialTime = 0f;

        dc = DiscourseContainer.Load(Path.Combine(Application.dataPath, "Scripts//CinemachineTimelineCode//FileIO//discourse.xml"));

        timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        director = GetComponent<PlayableDirector>();

        main_camera_object = GameObject.Find("Main Camera");

        ftrack = timeline.CreateTrack<CinemachineTrack>(null, "film_track");
        ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
        ntrack = timeline.CreateTrack<PlayableTrack>(null, "nav_track");
        ptrack = timeline.CreateTrack<PlayableTrack>(null, "timetravel_track");

        foreach (DiscourseClip clip in dc.Clips)
        {
            if (clip.Type.Equals("cam_custom"))
            {
                populateCustom(clip);
            }
            else if (clip.Type.Equals("cam_timeline"))
            {
                populateCamObject(clip);
            }
            else if (clip.Type.Equals("cntrl_timeline"))
            {
                populateCtrlObject(clip);
            }
            else
            {
                Debug.Log("What TYPE of discourse clip is this?");
            }
        }
        director.SetGenericBinding(ftrack, main_camera_object);
        director.Play(timeline);
    }

    private void populateCtrlObject(DiscourseClip clip)
    {
        agent = GameObject.Find(clip.TimeLineObject);
        
        control_track_clip = ctrack.CreateDefaultClip();
        control_track_clip.start = clip.start;
        control_track_clip.duration = clip.duration;
        control_track_clip.displayName = clip.Name;
        var controlAnim = control_track_clip.asset as ControlPlayableAsset;
        AnimateBind(controlAnim, agent);

        if (clip.fabulaStart >= 0f)
        {
            //time_track_clip = ptrack.CreateClip<setFloatClip>();
            time_track_clip = ptrack.CreateClip<setFloatClip>();
            time_track_clip.start = clip.start;
            time_track_clip.duration = clip.duration;
            //time_track_clip.duration = (double)0.05;
            time_track_clip.displayName = clip.Name;
            var timeControl = time_track_clip.asset as setFloatClip;
            
            TimeBind(timeControl, agent, clip.fabulaStart);
        }
    }

    // Update is called once per frame
    private void populateCustom(DiscourseClip clip)
    {
        // find aim target and follow Target
        agent = GameObject.Find(clip.aimTarget);
        anchor = GameObject.Find(clip.followTarget);

        Vector3 goal_direction = degToVector3(clip.targetOrientation + anchor.transform.eulerAngles.y);
        goal_direction = goal_direction.normalized * clip.targetDistance;
        
        // create position of camera
        GameObject go = new GameObject();
        go.name = "anchor";
        go.transform.position = anchor.transform.position + goal_direction + new Vector3(0f, HEIGHT, 0f);
        go.transform.parent = anchor.transform;

        // create position of target
        GameObject target_go = new GameObject();
        target_go.name = "target_obj";
        target_go.transform.position = agent.transform.position + new Vector3(0f, HEIGHT, 0f);
        target_go.transform.parent = agent.transform;

        // create vcam
        CinemachineVirtualCamera cva = go.AddComponent<CinemachineVirtualCamera>();
        cva.m_LookAt = target_go.transform;
        cva.m_Follow = go.transform;
        cva.m_Lens.FieldOfView = clip.fov;

        // default clip attributes
        film_track_clip = ftrack.CreateDefaultClip();
        film_track_clip.start = clip.start;
        film_track_clip.duration = clip.duration;
        film_track_clip.displayName = clip.Name;

        // specialize and bind
        var cinemachineShot = film_track_clip.asset as CinemachineShot;
        CamBind(cinemachineShot, cva);
    }

    private void populateCamObject(DiscourseClip clip)
    {
        new_cam = GameObject.Find(clip.TimeLineObject).GetComponent<CinemachineVirtualCamera>();

        film_track_clip = ftrack.CreateDefaultClip();
        film_track_clip.start = clip.start;
        film_track_clip.duration = clip.duration;
        film_track_clip.displayName = clip.Name;
        var cinemachineShot = film_track_clip.asset as CinemachineShot;
        CamBind(cinemachineShot, new_cam);
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

    private void TransformBind(LerpMoveObjectAsset tpObj, GameObject obj_to_move, Transform end_pos)
    {
        tpObj.ObjectToMove.exposedName = UnityEditor.GUID.Generate().ToString();
        tpObj.LerpMoveTo.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(tpObj.ObjectToMove.exposedName, obj_to_move);
        director.SetReferenceValue(tpObj.LerpMoveTo.exposedName, end_pos);
    }

    private void AnimateBind(ControlPlayableAsset cpa, GameObject ato)
    {
        cpa.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cpa.sourceGameObject.exposedName, ato);
    }

    private void TimeBind(setFloatClip sfc, GameObject host, float new_val)
    {
        sfc.target_cam.exposedName = UnityEditor.GUID.Generate().ToString();
        sfc.data.exposedName = UnityEditor.GUID.Generate().ToString();
        setFloatData sfd = agent.AddComponent<setFloatData>();
        sfd.new_value = new_val;
        director.SetReferenceValue(sfc.data.exposedName, sfd);
        //sfc.template
        //sfc.template.new_time = new_val;
        director.SetReferenceValue(sfc.target_cam.exposedName, host);
    }

    // orientation should be in degrees (i.e. oriention *= Mathf.Rad2Deg)
    private GameObject makeCustomizedTransform(Vector3 pos, float orientation)
    {
        GameObject t = new GameObject();
        t.transform.position = pos;
        t.transform.rotation = Quaternion.Euler(0f, orientation, 0f);
        return t;
    }

    private void CamBind(CinemachineShot cshot, CinemachineVirtualCamera vcam)
    {
        cshot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
        director.SetReferenceValue(cshot.VirtualCamera.exposedName, vcam);

    }

    public static Vector3 degToVector3(float degs)
    {
        float rads = Kinematic.mapToRange(degs*Mathf.Deg2Rad);
        return new Vector3(Mathf.Cos(rads), 0f, Mathf.Sin(rads));
    }
        
}
