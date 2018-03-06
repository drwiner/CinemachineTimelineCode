using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinematography;
using GoalNamespace;
using GraphNamespace;
using SteeringNamespace;

namespace ClipNamespace
{
    public class FabulaClip : Clip
    {
        public string animation_string;

        // findable game objects in scene
        public GameObject agent;
        public GameObject starting_location;
        public GameObject animTimelineObject;

        // timeline tracks
        public TrackAsset ctrack;

        // assets and clips
        public ControlPlayableAsset controlAnim;
        public TimelineClip control_track_clip;


        public FabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) : base(json, p_timeline, p_director)
        {

            ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");

            starting_location = GameObject.Find(json["start_pos_name"]);
            agent = GameObject.Find(json["gameobject_name"]);

            animation_string = json["animation_name"];
            step_id = json["step_id"];
            step_num = json["step_num"];

            // need to know if action is generic or specialized; for now, all generic
            if (animation_string != null)
            {
                SetAgentToGenericAction();
            }

            TimelineClip textSwitcherClip = TrackAttributes.fabTextTrack.CreateClip<TextSwitcherClip>();
            textSwitcherClip.start = start;
            textSwitcherClip.duration = duration;
            textSwitcherClip.displayName = Name;
            TextBind(textSwitcherClip.asset as TextSwitcherClip, "action: " + json["name"].Value + ", start: " + start.ToString() + ", duration: " + duration.ToString(), 16, Color.white);
        }

        public void SetAgentToGenericAction()
        {
            var animTimelineObject_template = GameObject.Find(animation_string);
            animTimelineObject = Object.Instantiate(animTimelineObject_template);
            var director01 = animTimelineObject.GetComponent<PlayableDirector>();
            var timeline01 = director01.playableAsset as TimelineAsset;

            foreach (var track in timeline01.GetOutputTracks())
            {
                var animTrack = track as AnimationTrack;
                if (animTrack == null)
                    continue;
                var binding = director01.GetGenericBinding(animTrack);
                if (binding == null)
                    continue;

                director01.SetGenericBinding(animTrack, agent.GetComponent<Animator>());
            }
        }

    }

    public class NavigateFabulaClip : FabulaClip
    {
        public TimelineClip nav_track_clip;

        public GameObject ending_location;

        public PlayableTrack ntrack;


        public NavigateFabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) : base(json, p_timeline, p_director)
        {
            ending_location = GameObject.Find(json["end_pos_name"]);

            Vector3 dest_minus_origin = ending_location.transform.position - starting_location.transform.position;
            float orientation = Mathf.Atan2(dest_minus_origin.z, -dest_minus_origin.x) * Mathf.Rad2Deg;

            var tempstart = new Vector3(starting_location.transform.position.x, agent.transform.position.y, starting_location.transform.position.z);
            Transform start_transform = MakeCustomizedTransform(tempstart, orientation).transform;

            var nav_track_clip = TrackAttributes.LerpTrackManager.CreateClip(start, duration, Name);

            nav_track_clip.start = start;
            nav_track_clip.duration = duration;
            LerpMoveObjectAsset lerp_clip = nav_track_clip.asset as LerpMoveObjectAsset;
            var tempend = new Vector3(ending_location.transform.position.x, agent.transform.position.y, ending_location.transform.position.z);
            Transform end_transform = MakeCustomizedTransform(tempend, orientation).transform;
            TransformBind(lerp_clip, agent, start_transform, end_transform);

            // control track - animate
            control_track_clip = ctrack.CreateDefaultClip();
            control_track_clip.start = start + 0.06f;
            control_track_clip.duration = duration - 0.06f;
            control_track_clip.displayName = Name;
            controlAnim = control_track_clip.asset as ControlPlayableAsset;
            AnimateBind(controlAnim, animTimelineObject);

        }
        

    }

    public class SteerFabulaClip : FabulaClip
    {
        public GameObject ending_location;
        private Node startNode, goalNode;
        private TileGraph TG;
        private Stack<Node> Path;
        private PlayableTrack ntrack;


        public SteerFabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) 
            : base(json, p_timeline, p_director)
        {
            ending_location = GameObject.Find(json["end_pos_name"].Value);

            // Quantize to find start and finish nodes
            startNode = QuantizeLocalize.Quantize(starting_location.transform.position, TrackAttributes.TG);
            goalNode = QuantizeLocalize.Quantize(ending_location.transform.position, TrackAttributes.TG);

            // Calculate path
            Path = PathFind.Dijkstra(TrackAttributes.TG, startNode, goalNode);

            // create Lerp for each edge in path
            var eachSeg = duration / Path.Count;
            int n = 0;
            var lastNode = startNode;
            foreach (Node p in Path)
            {
                ClipInfo CI = new ClipInfo(p_director, start + n*eachSeg, eachSeg, Name);
                CI.SimpleLerpClip(agent, lastNode.transform, p.transform);
                lastNode = p;
                n++;
            }

            // run animation across whole thing if not null
            if (json["animation_name"] != null)
            {
                control_track_clip = ctrack.CreateDefaultClip();
                control_track_clip.start = start + 0.06f;
                control_track_clip.duration = duration - 0.06f;
                control_track_clip.displayName = Name;
                controlAnim = control_track_clip.asset as ControlPlayableAsset;
                AnimateBind(controlAnim, animTimelineObject);
            }
        }
    }

    public class AISteerFabulaClip : FabulaClip
    {
        public GameObject ending_location;
        private Vector3 startNode, goalNode;

        public AISteerFabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director)
            : base(json, p_timeline, p_director)
        {
            ending_location = GameObject.Find(json["end_pos_name"].Value);

            ClipInfo CI = new ClipInfo(p_director, start, duration, Name);
            CI.SteerClip(agent, starting_location.transform.position, ending_location.transform.position, false, true);

            // number of segs should based on total path / slow radius
            int numSegs = 3;
            var lastPosition = starting_location.transform.position;
            var distance = ending_location.transform.position - lastPosition;
            var slowRadius = agent.GetComponent<DynoBehavior_TimelineControl>().slowRadius;
            numSegs = (int)Mathf.Ceil(distance.magnitude / slowRadius);
            var eachSeg = duration / numSegs;

            var directionSeg = distance / numSegs;
            for (int n = 0; n < numSegs; n++)
            {

                var p = lastPosition + directionSeg;

                if (n == 0)
                {
                    lastPosition = p;
                    continue;
                }
                CI = new ClipInfo(p_director, start + n * eachSeg, eachSeg, Name + n.ToString());
                CI.SteerClip(agent, p, ending_location.transform.position, true, true);
                lastPosition = p;
            }
        }
    }

    public class DroneSteerFabulaClip : FabulaClip
    {
        public GameObject ending_location;
        private Vector3 startNode, goalNode;

        public DroneSteerFabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director)
            : base(json, p_timeline, p_director)
        {
            ending_location = GameObject.Find(json["end_pos_name"].Value);

            // create Steer for each edge in path
            int numSegs = 4;
            var eachSeg = duration / numSegs;
            var lastPosition = starting_location.transform.position;
            var directionSeg = (ending_location.transform.position - lastPosition) / numSegs;


            // test 
            ClipInfo CI = new ClipInfo(p_director, start, duration, Name);
            CI.SteerClip(agent, starting_location.transform.position, ending_location.transform.position, true, true);
            // for each segment
            //for (int n = 0;  n < numSegs; n++)
            //{
            //    ClipInfo CI = new ClipInfo(p_director, start + n * eachSeg, eachSeg, Name);
            //    var p = lastPosition + directionSeg;
            //    // for test:
            //    //GameObject intermed = new GameObject();
            //    //intermed.name = n.ToString();
            //    //intermed.transform.position = p;
            //    if (n == 0)
            //    {
            //        CI.SteerClip(agent, lastPosition, p, true, false);
            //    }
            //    else if (n == numSegs - 1)
            //    {
            //        CI.SteerClip(agent, lastPosition, ending_location.transform.position, false, true);
            //    }
            //    else
            //    {
            //        CI.SteerClip(agent, lastPosition, p, false, false);
            //    }
            //    lastPosition = p;
            //}


        }
    }

    public class StationaryFabulaClip : FabulaClip
    {
        public StationaryFabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) : base(json, p_timeline, p_director)
        {
        }
    }

}