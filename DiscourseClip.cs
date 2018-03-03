using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine.Timeline;
using Cinemachine;
using Cinematography;
using System;

namespace ClipNamespace
{

    public class DiscourseClip : Clip
    {
        public float HEIGHT = 1.5f;
        public string animation_string;
        private string method_used;
        public float fab_start;

        // default fov is 40?
        public float fov = 40f;

        // default framing is Waist shot
        public FramingType frame_type = FramingType.Waist;
        public float orientation;
        public bool has_fab_switch;

        // tracks
        public PlayableTrack ptrack;
        public TrackAsset pos_track;

        // timeline clips
        public ControlPlayableAsset controlAnim;
        public TimelineClip nav_track_clip;
        public TimelineClip film_track_clip;


        public GameObject agent;
        public GameObject starting_location;
        public GameObject target_go;
        public GameObject host_go;

        public CinemachineVirtualCamera cva;
        
        public CinemachineComposer cc;
        public CinemachineBasicMultiChannelPerlin cbmcp;

        public DiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) : base(json, p_timeline, p_director)
        {

            fab_start = json["fab_start"].AsFloat;

            film_track_clip = TrackAttributes.FilmTrackManager.CreateClip(start, duration, Name);

            // create the time travel on fabula timeline
            createTimeClip();

            // create the target that the camera aims at
            createTarget(json);

            // create the camera and position
            createCameraObj(json);

            // create text UI
            createTextClip(json);

        }

        public void createTextClip(JSONNode json)
        {
            TimelineClip textSwitcherClip = TrackAttributes.discTextTrack.CreateClip<TextSwitcherClip>();
            textSwitcherClip.start = film_track_clip.start;
            textSwitcherClip.duration = film_track_clip.duration;
            textSwitcherClip.displayName = Name;
            var end_time = fab_start + duration;
            string message = "scale: " + json["scale"].Value + ", orient: " + json["orient"].AsFloat.ToString() + ", fabTimeSlice: [" + fab_start.ToString() + ": " + end_time.ToString() + "]";
            TextBind(textSwitcherClip.asset as TextSwitcherClip, message, 16, Color.white);

        }

        public void createTimeClip()
        {
            GameObject ttravel = GameObject.Instantiate(Resources.Load("time_travel", typeof(GameObject))) as GameObject;
            ttravel.GetComponent<timeStorage>().fab_time = fab_start;
            var tc = TrackAttributes.TimeTrackManager.CreateClip(start, duration, "Time Travel");
            var time_travel_clip = tc.asset as ControlPlayableAsset;
            AnimateBind(time_travel_clip, ttravel);
        }

        public void createTarget(JSONNode json)
        {
            agent = GameObject.Find(json["aim_target"]);
            BoxCollider bc = agent.GetComponent<BoxCollider>();
            // create position of target 
            target_go = new GameObject("target_" + Name);
            target_go.transform.position = agent.transform.position + new Vector3(0f, .8f, 0f);
            target_go.transform.parent = agent.transform;
            target_go.AddComponent<BoxCollider>();
            target_go.GetComponent<BoxCollider>().size = bc.size;
            target_go.GetComponent<BoxCollider>().center = bc.center;
        }

        public void createCameraObj(JSONNode json)
        {
            // set fov
            fov = (json["fov"] != null) ? json["fov"].AsFloat : fov;

            // create host for camera
            host_go = new GameObject("host_" + Name);

            // create Cinemachine component on host
            cva = host_go.AddComponent<CinemachineVirtualCamera>();
            cc = cva.AddCinemachineComponent<CinemachineComposer>();
            cc.m_DeadZoneWidth = 0.25f;
            cc.m_SoftZoneWidth = 0.5f;
            cva.m_Lens.FieldOfView = fov;
            cva.m_LookAt = target_go.transform;

            // create small amount of noise
            cbmcp = cva.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cbmcp.m_NoiseProfile = CinematographyAttributes.standardNoise;
            cbmcp.m_AmplitudeGain = 0.5f;
            cbmcp.m_FrequencyGain = 1f;

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

        public CustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
        {
            starting_location = GameObject.Find(json["start_pos_name"].Value);

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
 
        public NavCustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
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

            float orient = json["orient"].AsFloat;
            Vector3 dest_minus_start = (ending_location.transform.position - starting_location.transform.position).normalized;
            orientation = Mathf.Atan2(dest_minus_start.x, -dest_minus_start.z) * Mathf.Rad2Deg - 90f;

            Vector3 goal_direction = degToVector3(orient + orientation);
            
            Vector3 agent_starting_position = starting_location.transform.position + dest_minus_start * start_disc_offset;
            Vector3 agent_middle_position = agent_starting_position + dest_minus_start * (end_disc_offset / 2);

            if (json["scale"] != null)
            {
                var ft = json["scale"].Value;
                if (Enum.IsDefined(typeof(FramingType), ft))
                {
                    // cast var as FramingType
                    frame_type = (FramingType)Enum.Parse(typeof(FramingType), ft);
                }
            }

            float camDist = CinematographyAttributes.CalcCameraDistance(target_go, frame_type);

            Debug.Log("camera Distance: " + camDist.ToString());
            Debug.Log("goalDirection: " + (orient + orientation).ToString());

            host_go.transform.position = agent_middle_position + (goal_direction * camDist) + new Vector3(0f, HEIGHT, 0f);
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

        public VirtualDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
        {

            //assignCameraPosition(json);

            virtualCamOriginal = GameObject.Find(json["camera_name"].Value);
            virtualCam = GameObject.Instantiate(virtualCamOriginal);
            cva = virtualCam.GetComponent<CinemachineVirtualCamera>();

        }



    }

    public class NavVirtualDiscourseClip : VirtualDiscourseClip
    {
        public NavVirtualDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
        {
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

    }

    public class StationaryVirtualDiscourseClip : VirtualDiscourseClip
    {
        public StationaryVirtualDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
        {
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

    }

    
}