using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using UnityEngine.AI;

public class assign_anim_to_clip : MonoBehaviour {
    private TimelineAsset timeline;

    string p3 = "Assets//Scripts//CinemachineTimelineCode//xml_docs//anim_world.xml";

    List<List<string>> readFabula()
    {
        List<List<string>> clipList = new List<List<string>>();
        List<string> clipItems;

        XmlTextReader xml_doc = new XmlTextReader(p3);
        // cycle through each child noed 
        while (xml_doc.Read())
        {
            switch (xml_doc.NodeType)
            {
                case XmlNodeType.Element: // if the node is an element
                    clipItems = new List<string>();
                    while (xml_doc.MoveToNextAttribute())
                        clipItems.Add(xml_doc.Value);
                    //  Debug.Log("- " + xml_doc.Name + "='" + xml_doc.Value + "'");

                    if (clipItems.Count > 0)
                    {
                        clipList.Add(clipItems);
                    }
                    break;
            }
        }
        return clipList;
    }

    // Use this for initialization
    void Awake () {
        List<List<string>> clipList = readFabula();
        foreach (List<string> clipItem in clipList)
        {
            Debug.Log("Clip:");
            foreach (string item in clipItem)
            {
                Debug.Log(item);
            }
        }

        timeline = (TimelineAsset)ScriptableObject.CreateInstance("TimelineAsset");

        PlayableDirector director = GetComponent<PlayableDirector>();

        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");
        PlayableTrack ntrack = timeline.CreateTrack<PlayableTrack>(null, "nav_track");

        foreach (List<string> clipitem_list in clipList)
        {


            string name = clipitem_list[0];
            string type = clipitem_list[1];

            if (type.Equals("animate"))
            {
                float start = float.Parse(clipitem_list[2]);
                float dur = float.Parse(clipitem_list[3]);
                string start_location = clipitem_list[4];
                string end_location = clipitem_list[5];
                string animation_obj = clipitem_list[6];

                var clip = ctrack.CreateDefaultClip();
                clip.start = start;
                clip.duration = dur;
                clip.displayName = name;
                GameObject animTimeline = GameObject.Find(animation_obj);
                var controlAnim = clip.asset as ControlPlayableAsset;
                Vector3 start_pos = GameObject.Find(start_location).transform.position;
                Vector3 end_pos = GameObject.Find(end_location).transform.position;

                // SET ATTRIBUTES OF TARGET TIMELINE, a child of some gameobject is the convention

                //PlayableDirector anim_director = animTimeline.GetComponent<PlayableDirector>();
                //List<PlayableBinding> playable_list = anim_director.playableAsset.outputs.ToList();

                // this is the animation track, always; here, we are setting an offset
                //TrackAsset target_anim_track = playable_list[0].sourceObject as AnimationTrack;
                //List<TimelineClip> track_clips = target_anim_track.GetClips().ToList();
                //AnimationPlayableAsset target_anim_clip = track_clips[0].asset as AnimationPlayableAsset;
                //target_anim_clip.position = start_pos;

                // this is the lerp track, always; here, we set destination
                //TrackAsset target_lerp_track = playable_list[1].sourceObject as PlayableTrack;
                //List<TimelineClip> lerp_clips = target_lerp_track.GetClips().ToList();

                // Set control clip to be on fabula timeline
                controlAnim.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
                director.SetReferenceValue(controlAnim.sourceGameObject.exposedName, animTimeline);
            }
            else if(type.Equals("navigate")){
                float start = float.Parse(clipitem_list[2]);
                float dur = float.Parse(clipitem_list[3]);
                string start_location = clipitem_list[4];
                string end_location = clipitem_list[5];
                string agent = clipitem_list[6];


                //ntrack
                ////var clip = ctrack.CreateDefaultClip();
                var lerp_clip = ntrack.CreateClip<LerpMoveObjectAsset>();
                var clip = ntrack.CreateClip<SetAgentTargetAsset>();


                //var navclip = clip.asset as SetAgentTargetAsset;
                lerp_clip.start = start;
                lerp_clip.duration = .05f;
                lerp_clip.displayName = "lerp";
                
                clip.start = start + .05f;
                clip.duration = dur - .05f;
                clip.displayName = name;
                GameObject movingObj = GameObject.Find(agent);
                NavMeshAgent navigatingAgent = movingObj.GetComponent<NavMeshAgent>();

                Transform start_pos = GameObject.Find(start_location).transform;
                Transform end_pos = GameObject.Find(end_location).transform;

                var navclip = clip.asset as SetAgentTargetAsset;
                var lerpclip = lerp_clip.asset as LerpMoveObjectAsset;

               

                navclip.AgentSpeed = 2.0f;
                //navclip.Agent = navigatingAgent as NavMeshAgent;

                navclip.Target.exposedName = UnityEditor.GUID.Generate().ToString();
                navclip.Agent.exposedName = UnityEditor.GUID.Generate().ToString();
                director.SetReferenceValue(navclip.Agent.exposedName, navigatingAgent);
                director.SetReferenceValue(navclip.Target.exposedName, start_pos);
                lerpclip.ObjectToMove.exposedName = UnityEditor.GUID.Generate().ToString();
                lerpclip.LerpMoveTo.exposedName = UnityEditor.GUID.Generate().ToString();
                director.SetReferenceValue(lerpclip.ObjectToMove.exposedName, movingObj);
                director.SetReferenceValue(lerpclip.LerpMoveTo.exposedName, end_pos);
            }
            else
            {
                Debug.Log("incorrect clip type");
            }
           


        }

        director.Play(timeline);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
