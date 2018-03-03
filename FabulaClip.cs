using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinematography;

namespace ClipNamespace
{
    public class FabulaClip : Clip
    {
        public string animation_string;
        private string method_used;

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

            method_used = json["method_used"];
            step_id = json["step_id"];
            step_num = json["step_num"];

            // need to know if action is generic or specialized; for now, all generic
            SetAgentToGenericAction();

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

            ntrack = timeline.CreateTrack<PlayableTrack>(null, "nav_track");

            ending_location = GameObject.Find(json["end_pos_name"]);

            Vector3 dest_minus_origin = ending_location.transform.position - starting_location.transform.position;
            float orientation = Mathf.Atan2(dest_minus_origin.z, -dest_minus_origin.x) * Mathf.Rad2Deg;

            var tempstart = new Vector3(starting_location.transform.position.x, agent.transform.position.y, starting_location.transform.position.z);
            Transform start_transform = makeCustomizedTransform(tempstart, orientation).transform;

            nav_track_clip = ntrack.CreateClip<LerpMoveObjectAsset>();

            nav_track_clip.start = start;
            nav_track_clip.duration = duration;
            LerpMoveObjectAsset lerp_clip = nav_track_clip.asset as LerpMoveObjectAsset;
            var tempend = new Vector3(ending_location.transform.position.x, agent.transform.position.y, ending_location.transform.position.z);
            Transform end_transform = makeCustomizedTransform(tempend, orientation).transform;
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

    public class StationaryFabulaClip : FabulaClip
    {
        public StationaryFabulaClip(JSONNode json, TimelineAsset p_timeline, PlayableDirector p_director) : base(json, p_timeline, p_director)
        {
        }
    }

}