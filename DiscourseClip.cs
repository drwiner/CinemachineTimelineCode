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
        private Vector3 height_offset = Vector3.zero;

        // timeline clips
        public ControlPlayableAsset controlAnim;
        public TimelineClip nav_track_clip;
        public TimelineClip film_track_clip;

        // action parameters
        public GameObject agent;
        public GameObject starting_location;
        public float orientation;
        public string animation_string;
        //private string method_used;
        public float fab_start;
        // need to set this based on camera's target
        public float HEIGHT = 1.17f;

        // target gameobject is what the camera aims at
        public GameObject target_go;

        // host gameobject is what the camera sits on
        public GameObject host_go;

        // camera parameters
        public CinemachineVirtualCamera cva;
        public CinemachineComposer cc;
        public CinemachineBasicMultiChannelPerlin cbmcp;
        public CinemachineCameraBody cbod;
        private FramingParameters framing_data;
        public FramingType frame_type = FramingType.Waist;
     

        public DiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) : base(json, p_timeline, p_director)
        {

            fab_start = json["fab_start"].AsFloat;

            film_track_clip = TrackAttributes.FilmTrackManager.CreateClip(start, duration, Name);

            // create the time travel on fabula timeline
            CreateTimeClip();

            // create the target that the camera aims at
            CreateTarget(json);

            // create the camera and position
            CreateCameraObj(json);

            // create text UI
            CreateTextClip(json);

        }

        public void CreateFrameType(JSONNode json)
        {
            var ft = json["scale"].Value;
            if (Enum.IsDefined(typeof(FramingType), ft))
            {
                // cast var as FramingType
                frame_type = (FramingType)Enum.Parse(typeof(FramingType), ft);
            }

            framing_data = FramingParameters.FramingTable[frame_type];
        }

        /// <summary>
        /// Text Display Timeline Clip
        /// </summary>
        /// <param name="json"></param>
        public virtual void CreateTextClip(JSONNode json)
        {
            TimelineClip textSwitcherClip = TrackAttributes.discTextTrack.CreateClip<TextSwitcherClip>();
            AssignClipAttributes(film_track_clip, textSwitcherClip, Name);
            var end_time = fab_start + duration;
            string message = "scale: " + json["scale"].Value + ", orient: " + json["orient"].AsFloat.ToString() + ", fabTimeSlice: [" + fab_start.ToString() + ": " + end_time.ToString() + "]";
            TextBind(textSwitcherClip.asset as TextSwitcherClip, message, 16, Color.white);

        }

        public void CreateTimeClip()
        {
            GameObject ttravel = GameObject.Instantiate(Resources.Load("time_travel", typeof(GameObject))) as GameObject;
            ttravel.GetComponent<timeStorage>().fab_time = fab_start;
            var tc = TrackAttributes.TimeTrackManager.CreateClip(start, duration, "Time Travel");
            var time_travel_clip = tc.asset as ControlPlayableAsset;
            AnimateBind(time_travel_clip, ttravel);
        }

        public void CreateTarget(JSONNode json)
        {
            agent = GameObject.Find(json["aim_target"]);
            BoxCollider bc = agent.GetComponent<BoxCollider>();
            // create position of target 
            target_go = new GameObject("target_" + Name);
            target_go.transform.position = agent.transform.position + height_offset;
            target_go.transform.parent = agent.transform;
            target_go.AddComponent<BoxCollider>();
            target_go.GetComponent<BoxCollider>().size = bc.size;
            //target_go.GetComponent<BoxCollider>().center = bc.center;
        }

        public void CreateCameraObj(JSONNode json)
        {
            CreateFrameType(json);
            // create host for camera
            host_go = new GameObject("host_" + Name);

            // create Cinemachine component on host
            cva = host_go.AddComponent<CinemachineVirtualCamera>();
            cc = cva.AddCinemachineComponent<CinemachineComposer>();
            cc.m_DeadZoneWidth = 0.25f;
            cc.m_SoftZoneWidth = 0.5f;
            //cva.m_Lens.FieldOfView = CinematographyAttributes.calcFov(frame_type);
            cva.m_LookAt = target_go.transform;

            cbod = host_go.AddComponent<CinemachineCameraBody>();
            // FStop
            cbod.IndexOfFStop = CinematographyAttributes.fStops[framing_data.DefaultFStop];
            // Lens
            cbod.IndexOfLens = CinematographyAttributes.lenses[framing_data.DefaultFocalLength];
            
            // create small amount of noise
            cbmcp = cva.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cbmcp.m_NoiseProfile = CinematographyAttributes.standardNoise;
            cbmcp.m_AmplitudeGain = 0.5f;
            cbmcp.m_FrequencyGain = 1f;

            //var clearShot = host_go.AddComponent<CinemachineClearShot>();
            
            //clearShot.ChildCameras
        }

        public virtual void AssignCameraPosition(JSONNode json)
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

        public static Vector3 DegToVector3(float degs)
        {
            float rads = MapToRange(degs * Mathf.Deg2Rad);
            return new Vector3(Mathf.Cos(rads), 0f, Mathf.Sin(rads));
        }

        public static float FindNearPlane(Vector3 origin, Vector3 direction, float dist)
        {
            var n = 1;
            while (true)
            {
                Debug.Log("ray casting");
                n++;
                if (!Physics.Raycast(origin, -direction, dist - n))
                {
                    break;
                }
            }
           return n;
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

        }

        public void CreateClearShot()
        {
            Debug.Log("startingClearShotConstruction");
            var hostClone = GameObject.Instantiate(host_go);
            
            host_go.GetComponent<CinemachineVirtualCamera>().enabled = false;
            host_go.GetComponent<CinemachineCameraBody>().enabled = false;
            var ccs = host_go.AddComponent<CinemachineClearShot>();
            ccs.m_MinDuration = 1.5f;
            ccs.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
           
            var ccollider = host_go.AddComponent<CinemachineCollider>();
            ccollider.m_Damping = 5;
            //ccollider.m_
            //ccollider.m_Strategy = CinemachineCollider.ResolutionStrategy.
            var origPos = host_go.transform.position;


            var clearShotPositions = new List<Vector3>() {
                    origPos,
                    new Vector3(origPos.x - 2f, origPos.y, origPos.z),
                    new Vector3(origPos.x + 2f, origPos.y, origPos.z),
                    new Vector3(origPos.x, origPos.y, origPos.z + 2f),
                    new Vector3(origPos.x, origPos.y, origPos.z - 2f),
                    new Vector3(origPos.x + 2f, origPos.y, origPos.z + 2f),
                    new Vector3(origPos.x + 2f, origPos.y, origPos.z - 2f),
                    new Vector3(origPos.x - 2f, origPos.y, origPos.z + 2f),
                    new Vector3(origPos.x - 2f, origPos.y, origPos.z - 2f),
                    new Vector3(origPos.x - 6f, origPos.y, origPos.z),
                    new Vector3(origPos.x + 6f, origPos.y, origPos.z),
                    new Vector3(origPos.x, origPos.y, origPos.z + 6f),
                    new Vector3(origPos.x, origPos.y, origPos.z - 6f)
            };

            var cvaList = new CinemachineVirtualCamera[clearShotPositions.Count];
            
            for (int i = 0; i < clearShotPositions.Count; i++)
            {
                cvaList[i] = CreateClearShot(hostClone, clearShotPositions[i]);
            }
            ccs.m_ChildCameras = cvaList;
            cvaList[0].m_Priority = 11;

            Debug.Log("endingClearShotConstruction");
        }

        private CinemachineVirtualCamera CreateClearShot(GameObject hostClone, Vector3 variantPosition)
        {
            var clearCam = GameObject.Instantiate(hostClone);
            clearCam.transform.parent = host_go.transform;
            clearCam.transform.position = variantPosition;
            
            return clearCam.GetComponent<CinemachineVirtualCamera>();
        }
    }

    public class NavCustomDiscourseClip : CustomDiscourseClip
    {
        public GameObject ending_location;

        public PlayableTrack ntrack;

        private Vector3 agentStartPosition, agentMidPosition, agentEndPosition;

        private GameObject focusTarget;

        public float orient;
 
        public NavCustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
        {
 
            ending_location = GameObject.Find(json["end_pos_name"].Value);

            // these are used as a distance thing in custom 
            start_disc_offset = json["start_dist_offset"].AsFloat;
            end_disc_offset = json["end_dist_offset"].AsFloat;
            
            AssignCameraPosition(json);
       
            // specialize and bind
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

        public override void AssignCameraPosition(JSONNode json)
        {

            float orient = json["orient"].AsFloat;
            Vector3 dest_minus_start = (ending_location.transform.position - starting_location.transform.position).normalized;
            orientation = Mathf.Atan2(dest_minus_start.x, -dest_minus_start.z) * Mathf.Rad2Deg - 90f;

            var goal_direction = DegToVector3(orient + orientation);
            agentStartPosition = starting_location.transform.position + dest_minus_start * start_disc_offset;
            agentEndPosition = starting_location.transform.position + dest_minus_start * end_disc_offset;
            agentMidPosition = agentStartPosition + (agentEndPosition - agentStartPosition)/2;
            //Debug.Log(agentStartPosition.ToString() + agentEndPosition.ToString() + agentMidPosition.ToString());
            focusTarget = new GameObject();
            focusTarget.transform.position = agentMidPosition;
            var bc = focusTarget.AddComponent<BoxCollider>();
            bc.size = agent.GetComponent<BoxCollider>().size;

            float camDist = CinematographyAttributes.CalcCameraDistance(focusTarget, frame_type);

            //Debug.Log("camera Distance: " + camDist.ToString());
            //Debug.Log("goalDirection: " + (orient + orientation).ToString());

            cbod.FocusDistance = camDist;

            var camHeight = .75f * bc.size.y;

            // position of camera's height depends on angle
            host_go.transform.position = agentMidPosition + (goal_direction * camDist) + new Vector3(0f, camHeight, 0f); ;
                //+ new Vector3(0f, target_go.transform.position.y, 0f);
            host_go.transform.rotation.SetLookRotation(agentMidPosition);

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

            //var fwd = host_go.transform.TransformDirection(Vector3.forward);

            if (Physics.Raycast(target_go.transform.position, -goal_direction, camDist-2))
            {
                cva.m_Lens.NearClipPlane = FindNearPlane(target_go.transform.position, goal_direction, camDist);
            }

            // create clear shot 8 cameras
            
            CreateClearShot();
            
        }

        

        public override void CreateTextClip(JSONNode json)
        {
            TimelineClip textSwitcherClip = TrackAttributes.discTextTrack.CreateClip<TextSwitcherClip>();
            AssignClipAttributes(film_track_clip, textSwitcherClip, Name);
            var end_time = fab_start + duration;
            string message = "scale: " + json["scale"].Value + ", orient: " + json["orient"].AsFloat.ToString() + ", fabTimeSlice: [" + fab_start.ToString() + ": " + end_time.ToString() + "]";
            TextBind(textSwitcherClip.asset as TextSwitcherClip, message, 16, Color.white);

        }
    }

    public class SimpleCustomDiscourseClip : CustomDiscourseClip
    {
        public float orient;
        private float agentOrient = 0f;
        //private float camDist;

        public SimpleCustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director)
            : base(json, p_timeline, p_director)
        {

            AssignCameraPosition(json);

            AssignCameraHeight(json);

            // specialize and bind
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

        public override void AssignCameraPosition(JSONNode json)
        {

            orient = json["orient"].AsFloat;
            if (json["agentOrient"] != null)
                agentOrient = json["agentOrient"].AsFloat;

            var camDist = CinematographyAttributes.CalcCameraDistance(target_go, frame_type);

            cbod.FocusDistance = camDist;
            var goalDirection = DegToVector3(orient + agentOrient);
            host_go.transform.position = target_go.transform.position + goalDirection * camDist;
            host_go.transform.rotation.SetLookRotation(starting_location.transform.position);

            if (Physics.Raycast(target_go.transform.position, -goalDirection, camDist - 2))
            {
                cva.m_Lens.NearClipPlane = FindNearPlane(target_go.transform.position, goalDirection, camDist);
            }

            CreateClearShot();
        }

        public void AssignCameraHeight(JSONNode json)
        {
            var alpha = json["angle"].AsFloat;
            var height = CinematographyAttributes.SolveForY(target_go.transform.position, host_go.transform.position, alpha);
            host_go.transform.position = new Vector3(host_go.transform.position.x, host_go.transform.position.y+height, host_go.transform.position.z);
        }

    }

    public class TwoShotCustomDiscourseClip : CustomDiscourseClip
    {
        public float orient;
        private float agentOrient = 0f;
        private GameObject OtherTarget;

        public TwoShotCustomDiscourseClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director)
            : base(json, p_timeline, p_director)
        {

            OtherTarget = GameObject.Find(json["SecondTarget"].Value);
            AssignCameraPosition(json);

            // specialize and bind
            var cinemachineShot = film_track_clip.asset as CinemachineShot;
            CamBind(cinemachineShot, cva);
        }

        public override void AssignCameraPosition(JSONNode json)
        {

            orient = json["orient"].AsFloat;
            if (json["agentOrient"] != null)
                agentOrient = json["agentOrient"].AsFloat;

            var ctg = target_go.AddComponent<CinemachineTargetGroup>();
            var ctgt = new CinemachineTargetGroup.Target();
            ctgt.target = starting_location.transform;
            ctgt.weight = 1f;
            var ctgt2 = new CinemachineTargetGroup.Target();
            ctgt2.target = OtherTarget.transform;
            ctgt2.weight = 1f;
            ctg.m_Targets = new CinemachineTargetGroup.Target[] { ctgt, ctgt2 };

            // this line worked:
            //var directionTowardsSecondTarget = OtherTarget.transform.position - target_go.transform.position;
            //target_go.transform.position = target_go.transform.position + new Vector3(directionTowardsSecondTarget.x, 0f, directionTowardsSecondTarget.z)  / 2;

            float camDist = CinematographyAttributes.CalcCameraDistance(target_go, frame_type);
            //cva.m_LookAt = ctg.transform;

            Debug.Log("camera Distance: " + camDist.ToString());
            Debug.Log("goalDirection: " + (orient).ToString());

            cbod.FocusDistance = camDist;
            var goalDirection = DegToVector3(orient + agentOrient);
            host_go.transform.position = target_go.transform.position + goalDirection * camDist;
            host_go.transform.rotation.SetLookRotation(starting_location.transform.position);

            if (Physics.Raycast(target_go.transform.position, -goalDirection, camDist - 2))
            {
                cva.m_Lens.NearClipPlane = FindNearPlane(target_go.transform.position, goalDirection, camDist);
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