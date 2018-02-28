using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine.Timeline;
using Cinemachine;

namespace ClipNamespace
{

    public class DiscourseClip : Clip
    {
        public float HEIGHT = 1.5f;
        public string animation_string;
        private string method_used;
        public float fab_start;
        public float fov = 40f;
        public float orientation;
        public bool has_fab_switch;
       

        // tracks
        public TrackAsset ctrack;
        public CinemachineTrack ftrack;
        public PlayableTrack ptrack;
        public TrackAsset pos_track;

        // timeline clips
        public ControlPlayableAsset controlAnim;
        public TimelineClip control_track_clip;
        public TimelineClip nav_track_clip;
        public TimelineClip film_track_clip;
        public TimelineClip time_track_clip;
        public TimelineClip pos_track_clip;

        public GameObject agent;
        public GameObject starting_location;
        public GameObject target_go;
        public GameObject host_go;

        public CinemachineVirtualCamera cva;
        
        public CinemachineComposer cc;
        public CinemachineBasicMultiChannelPerlin cbmcp;


        public DiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director, CinemachineTrack ftrack) : base(json, p_timeline, p_director)
        {
            //main_camera_object = GameObject.Find("Main Camera");
            

            fab_start = json["fabulaStart"].AsFloat;

            ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
            //ftrack = timeline.CreateTrack<CinemachineTrack>(null, "film_track");

            starting_location = GameObject.Find(json["start_pos_name"].Value);

            if (json["fov"] != null)
            {
                fov = json["fov"].AsFloat;
            }

            has_fab_switch = false;

            // all discourse steps have fab switches
            if (fab_start >= 0f)
            {
                has_fab_switch = true;
                GameObject ttravel = Object.Instantiate(Resources.Load("time_travel", typeof(GameObject))) as GameObject;
                ttravel.GetComponent<timeStorage>().fab_time = fab_start;
                TimelineClip tc = ctrack.CreateDefaultClip();
                tc.start = start;
                tc.duration = duration;
                tc.displayName = "Time Travel";
                var time_travel_clip = tc.asset as ControlPlayableAsset;
                AnimateBind(time_travel_clip, ttravel);
            }

            agent = GameObject.Find(json["aim_target"]);

            // create position of target 
            target_go = new GameObject("target_" + Name);
            target_go.transform.position = agent.transform.position + new Vector3(0f, HEIGHT, 0f);
            target_go.transform.parent = agent.transform;

            // create host for camera
            host_go = new GameObject("host_" + Name);
            cva = host_go.AddComponent<CinemachineVirtualCamera>();
            cc = cva.AddCinemachineComponent<CinemachineComposer>();
            cc.m_DeadZoneWidth = 0.5f;
            cc.m_SoftZoneWidth = 0.8f;
            cva.m_Lens.FieldOfView = fov;
            cva.m_LookAt = target_go.transform;
            cbmcp = cva.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cbmcp.m_NoiseProfile = Object.Instantiate(Resources.Load("Handheld_tele_mild", typeof(NoiseSettings))) as NoiseSettings;
            cbmcp.m_AmplitudeGain = 0.5f;
            cbmcp.m_FrequencyGain = 1f;

            // where to position host_go? delegated to sub-class members
            //assignCameraPosition(json);
            

                // add camera behavior to film track
            film_track_clip = ftrack.CreateDefaultClip();
            float film_clip_start = start;
            float film_clip_duration = duration;
            if (has_fab_switch)
            {
                film_clip_start = start + (float)0.06;
                film_clip_duration = duration; // could consider pruning by 0.06;
            }

            film_track_clip.start = film_clip_start;
            film_track_clip.duration = film_clip_duration;
            film_track_clip.displayName = Name;

            
        }

        public virtual void assignCameraPosition(JSONNode json)
        {
            Debug.Log("Assign camera position -- method not implemented.");
            throw new System.Exception();
        }

        public void TimeBind(setFloatClip sfc, GameObject host, float new_val)
        {
            sfc.target_cam.exposedName = UnityEditor.GUID.Generate().ToString();
            sfc.data.exposedName = UnityEditor.GUID.Generate().ToString();
            setFloatData sfd = agent.AddComponent<setFloatData>();
            sfd.new_value = new_val;
            director.SetReferenceValue(sfc.data.exposedName, sfd);
            director.SetReferenceValue(sfc.target_cam.exposedName, host);
        }

        public void CamBind(CinemachineShot cshot, CinemachineVirtualCamera vcam)
        {
            cshot.VirtualCamera.exposedName = UnityEditor.GUID.Generate().ToString();
            director.SetReferenceValue(cshot.VirtualCamera.exposedName, vcam);

        }

        public static Vector3 degToVector3(float degs)
        {
            float rads = Kinematic.mapToRange(degs * Mathf.Deg2Rad);
            return new Vector3(Mathf.Cos(rads), 0f, Mathf.Sin(rads));
        }
    }

    /////////////////////// CUSTOM ///////////////////////

    public class CustomDiscourseClip : DiscourseClip
    {
        
        public float start_disc_offset;
        public float end_disc_offset;

        public CustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director, CinemachineTrack ftrack) 
            : base(json, p_timeline, p_director, ftrack)
        {


            // specialize and bind
            //var cinemachineShot = film_track_clip.asset as CinemachineShot;
            //CamBind(cinemachineShot, cva);

        }

        //public override void assignCameraPosition(JSONNode json)
        //{

        //}
    }

    public class NavCustomDiscourseClip : CustomDiscourseClip
    {
        public GameObject ending_location;

        public PlayableTrack ntrack;

        public float orient;
 
        public NavCustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director, CinemachineTrack ftrack) 
            : base(json, p_timeline, p_director, ftrack)
        {
            ending_location = GameObject.Find(json["end_pos_name"].Value);

            // these are used as a distance thing in custom 
            start_disc_offset = json["start_dist_offset"].AsFloat;
            end_disc_offset = json["end_dist_offset"].AsFloat;


            //json["target_orientation"]
            

            assignCameraPosition(json);


            // specialize and bind
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);

        }

        public override void assignCameraPosition(JSONNode json)
        {
            float orient = json["target_orientation"].AsFloat;
            Vector3 dest_minus_start = (ending_location.transform.position - starting_location.transform.position).normalized;
            orientation = Mathf.Atan2(dest_minus_start.x, -dest_minus_start.z) * Mathf.Rad2Deg - 90f;

            Vector3 goal_direction = degToVector3(orient + orientation);
            
            Vector3 agent_starting_position = starting_location.transform.position + dest_minus_start * start_disc_offset;
            Vector3 agent_middle_position = agent_starting_position + dest_minus_start * (end_disc_offset / 2);


            host_go.transform.position = agent_middle_position + (goal_direction * json["target_distance"].AsFloat) + new Vector3(0f, HEIGHT, 0f);
            host_go.transform.rotation.SetLookRotation(agent_starting_position);

            if (json["follow_target"] != null)
            {
                nav_track_clip = ntrack.CreateClip<LerpMoveObjectAsset>();
                nav_track_clip.start = start + 0.16f;
                nav_track_clip.duration = duration - 0.16f;
                LerpMoveObjectAsset lerp_clip = nav_track_clip.asset as LerpMoveObjectAsset;
                GameObject camera_destination = new GameObject();
                camera_destination.name = "camera lerp destination";
                Vector3 end_camera = ending_location.transform.position + (goal_direction * json["target_distance"].AsFloat) + new Vector3(0f, HEIGHT, 0f);
                camera_destination.transform.position = end_camera;
                GameObject camera_origin = new GameObject();
                camera_origin.name = "camera lerp origin";
                camera_origin.transform.position = host_go.transform.position;
                TransformBind(lerp_clip, host_go, camera_origin.transform, camera_destination.transform);
            }
        }
    }

    /////////////////////// VIRTUAL ///////////////////////
    public class VirtualDiscourseClip : DiscourseClip
    {
        public GameObject virtualCamOriginal;
        public GameObject virtualCam;

        public VirtualDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director, CinemachineTrack ftrack) 
            : base(json, p_timeline, p_director, ftrack)
        {

            //assignCameraPosition(json);

            virtualCamOriginal = GameObject.Find(json["camera_name"].Value);
            virtualCam = Object.Instantiate(virtualCamOriginal);
            cva = virtualCam.GetComponent<CinemachineVirtualCamera>();

        }



    }

    public class NavVirtualDiscourseClip : VirtualDiscourseClip
    {
        public NavVirtualDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director, CinemachineTrack ftrack) 
            : base(json, p_timeline, p_director, ftrack)
        {
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

    }

    public class StationaryVirtualDiscourseClip : VirtualDiscourseClip
    {
        public StationaryVirtualDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director, CinemachineTrack ftrack) 
            : base(json, p_timeline, p_director, ftrack)
        {
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

    }
}