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
    private TrackAsset pos_track;

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
    private TimelineClip pos_track_clip;

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
        pos_track = timeline.CreateTrack<ControlTrack>(null, "pos_track");

        foreach (DiscourseClip clip in dc.Clips)
        {
            if (clip.Type.Equals("cam_custom"))
            {
                populateCustom(clip);
            }
            else if (clip.Type.Equals("nav_cam"))
            {
                populateNav(clip);
            }
            // cam_timeline cannot time travel
            else if (clip.Type.Equals("cam_timeline"))
            {
                populateCamObject(clip);
            }
            else if (clip.Type.Equals("cntrl_timeline"))
            {
                populateCtrlObject(clip);
                if (clip.fabulaStart >= 0f)
                {
                    addTimeTravel(clip);
                }
            }
            else
            {
                Debug.Log("What TYPE of discourse clip is this?");
            }
            
        }
        director.SetGenericBinding(ftrack, main_camera_object);
        director.Play(timeline);
    }

    private void addTimeTravel(DiscourseClip clip)
    {

        time_track_clip = ptrack.CreateClip<setFloatClip>();
        time_track_clip.start = clip.start;
        time_track_clip.duration = clip.duration;
        time_track_clip.displayName = clip.Name;
        var timeControl = time_track_clip.asset as setFloatClip;

        TimeBind(timeControl, agent, clip.fabulaStart);
    }

    private void populateNav(DiscourseClip clip)
    {
        string name = clip.Name;
        float start = clip.start + 0.06f;
        float duration = clip.duration - 0.06f;
        float fov = clip.fov;

        float fab_start = clip.fabulaStart;
    
        GameObject starting_loc = GameObject.Find(clip.startingPos_string);
        float start_offset = clip.start_dist_offset;
        GameObject ending_loc = GameObject.Find(clip.endingPos_string);
        float end_offset = clip.end_dist_offset;

        float orient = clip.targetOrientation;
        agent = GameObject.Find(clip.aimTarget);
       

        // create position of target
        GameObject target_go = new GameObject();
        target_go.name = "target_" + clip.Name;
        target_go.transform.position = agent.transform.position + new Vector3(0f, HEIGHT, 0f);
        target_go.transform.parent = agent.transform;

        Vector3 dest_minus_start = (ending_loc.transform.position - starting_loc.transform.position).normalized;
        //dest_minus_start.Normalize();
        Vector3 agent_starting_position = starting_loc.transform.position + dest_minus_start * start_offset;
        Vector3 agent_middle_position = agent_starting_position + dest_minus_start * (end_offset/2);

        GameObject go = new GameObject();
        go.name = clip.Name;
        float orientation = Mathf.Atan2(dest_minus_start.x, -dest_minus_start.z) * Mathf.Rad2Deg - 90f;

        Vector3 goal_direction = degToVector3(orient + orientation);
        //Debug.Log(orient + orientation);

        go.transform.position = agent_middle_position + (goal_direction * clip.targetDistance) + new Vector3(0f, HEIGHT, 0f);
        go.transform.rotation.SetLookRotation(agent_starting_position);

        CinemachineVirtualCamera cva = go.AddComponent<CinemachineVirtualCamera>();
        CinemachineComposer cc = cva.AddCinemachineComponent<CinemachineComposer>();
        cc.m_DeadZoneWidth = 0.5f;
        cc.m_SoftZoneWidth = 0.8f;
        cva.m_Lens.FieldOfView = clip.fov;
        cva.m_LookAt = target_go.transform;
        

        CinemachineBasicMultiChannelPerlin cbmcp = cva.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cbmcp.m_NoiseProfile = Instantiate(Resources.Load("Handheld_tele_mild", typeof(NoiseSettings))) as NoiseSettings;
        cbmcp.m_AmplitudeGain = 0.5f;
        cbmcp.m_FrequencyGain = 1f;
        //NoiseSettings ns = cva.AddCinemachineComponent<NoiseSettings>();
        if (clip.followTarget != null)
        {
            nav_track_clip = ntrack.CreateClip<LerpMoveObjectAsset>();
            nav_track_clip.start = clip.start + 0.16f;
            nav_track_clip.duration = clip.duration - 0.16f;
            LerpMoveObjectAsset lerp_clip = nav_track_clip.asset as LerpMoveObjectAsset;
            GameObject camera_destination = new GameObject();
            camera_destination.name = "camera lerp destination";
            Vector3 end_camera = ending_loc.transform.position + (goal_direction * clip.targetDistance) + new Vector3(0f, HEIGHT, 0f);
            camera_destination.transform.position = end_camera;
            GameObject camera_origin = new GameObject();
            camera_origin.name = "camera lerp origin";
            camera_origin.transform.position = go.transform.position;
            TransformBind(lerp_clip, go, camera_origin.transform, camera_destination.transform);
        }


        bool has_fab_switch = false;
        if (clip.fabulaStart >= 0f)
        {
            has_fab_switch = true;
            GameObject ttravel = Instantiate(Resources.Load("time_travel", typeof(GameObject))) as GameObject;
            ttravel.transform.parent = go.transform;
            ttravel.GetComponent<timeStorage>().fab_time = clip.fabulaStart;
            TimelineClip tc = ctrack.CreateDefaultClip();
            tc.start = clip.start;
            tc.duration = clip.duration;
            tc.displayName = "Time Travel";
            var time_travel_clip = tc.asset as ControlPlayableAsset;
            AnimateBind(time_travel_clip, ttravel);
        }

        // default clip attributes
        film_track_clip = ftrack.CreateDefaultClip();
        if (has_fab_switch)
        {
            start = clip.start + (float)0.06;
            duration = clip.duration;
        }
        film_track_clip.start = start;
        film_track_clip.duration = duration;
        film_track_clip.displayName = clip.Name;

        // specialize and bind
        var cinemachineShot = film_track_clip.asset as CinemachineShot;
        CamBind(cinemachineShot, cva);

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
    }

    // Update is called once per frame
    private void populateCustom(DiscourseClip clip)
    {
        // find aim target and follow Target
        agent = GameObject.Find(clip.aimTarget);
        anchor = GameObject.Find(clip.followTarget);
        if (anchor == null)
        {
            // This "anchor" hosts the actual anchor
            anchor = new GameObject();
            anchor.name = clip.Name;
            GameObject VC_Host = GameObject.Find("VirtualCams");
            anchor.transform.position = new Vector3(0f, 0f, 0f);
            anchor.transform.parent = VC_Host.transform;
            //anchor = GameObject.Find(clip.followTarget);

            //anchor = agent;
        }

        // create position of target
        GameObject target_go = new GameObject();
        target_go.name = "target_" + clip.Name;
        target_go.transform.position = agent.transform.position + new Vector3(0f, HEIGHT, 0f);
        target_go.transform.parent = agent.transform;

        Vector3 goal_direction = degToVector3(clip.targetOrientation + agent.transform.eulerAngles.y);
        goal_direction = goal_direction.normalized * clip.targetDistance;
        
        // create position of camera
        GameObject go = new GameObject();
        go.name = clip.Name;
        go.transform.position = agent.transform.position + goal_direction + new Vector3(0f, HEIGHT, 0f);
        go.transform.parent = anchor.transform;


        // create vcam
        CinemachineVirtualCamera cva = go.AddComponent<CinemachineVirtualCamera>();
        cva.m_LookAt = target_go.transform;
        cva.m_Follow = go.transform;
        cva.m_Lens.FieldOfView = clip.fov;


        bool has_fab_switch = false;
        if (clip.fabulaStart >= 0f)
        {
            has_fab_switch = true;            
            //Instantiate(Resources.Load("enemy", typeof(GameObject))) as GameObject;
            GameObject ttravel = Instantiate(Resources.Load("time_travel", typeof(GameObject))) as GameObject;
            ttravel.transform.parent = go.transform;
            ttravel.GetComponent<timeStorage>().fab_time = clip.fabulaStart;


            TimelineClip tc = ctrack.CreateDefaultClip();
            tc.start = clip.start;
            tc.duration = clip.duration;
            tc.displayName = "Time Travel";
            var time_travel_clip = tc.asset as ControlPlayableAsset;

            AnimateBind(time_travel_clip, ttravel);

            //time_track_clip = ptrack.CreateClip<setFloatClip>();
            //time_track_clip.start = clip.start;
            //time_track_clip.duration = clip.duration;
            //time_track_clip.displayName = "Time Travel";
            //time_track_clip.displayName = clip.Name;
            //var timeControl = time_track_clip.asset as setFloatClip;

            //TimeBind(timeControl, ttravel, clip.fabulaStart);


            //ttravel

            ////gopd.
            ////gopd.playableAsset =
            ////TimelineAsset ta = Instantiate(Resources.Load("time_TravelTimeline", typeof(Playable))) as TimelineAsset;
            //TimelineAsset ta = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");
            //ActivationTrack atrack = ta.CreateTrack<ActivationTrack>(null, "local_active_track");
            //TimelineClip tc = atrack.CreateDefaultClip();
            //tc.start = 0f;
            //tc.duration = clip.duration;

            //GameObject son = ttravel.GetComponent<timeStorage>().setFabObj;
            ////tc.asset = son;
            //tc.underlyingAsset = son;
            //var activation_clip = tc.asset as ActivationTrack;
            ////activation_clip.postPlaybackState = ActivationTrack.PostPlaybackState.Inactive;

            ////activation_clip.parent = son;
            //ttravel.transform.parent = go.transform;
        }

        // default clip attributes
        film_track_clip = ftrack.CreateDefaultClip();
        float start = clip.start;
        float duration = clip.duration;
        if (has_fab_switch)
        {
            //start = clip.start + (float)0.16;
            //duration = clip.duration - (float)0.16;
            start = clip.start + (float)0.06;
            duration = clip.duration;
        } else
        {

        }
        film_track_clip.start = start;
        film_track_clip.duration = duration;
        film_track_clip.displayName = clip.Name;


        // specialize and bind
        var cinemachineShot = film_track_clip.asset as CinemachineShot;
        CamBind(cinemachineShot, cva);
    }

    private void populateCamObject(DiscourseClip clip)
    {
        agent = GameObject.Find(clip.TimeLineObject);
        new_cam = agent.GetComponent<CinemachineVirtualCamera>();

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
