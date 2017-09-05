using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

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

        TrackAsset track = timeline.CreateTrack<AnimationTrack>(null, "trackname");
        TrackAsset ctrack = timeline.CreateTrack<ControlTrack>(null, "control_track");

        foreach (List<string> clipitem_list in clipList)
        {
            string name = clipitem_list[0];
            string type = clipitem_list[1];
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
            PlayableDirector anim_director = animTimeline.GetComponent<PlayableDirector>();
            List<PlayableBinding> playable_list = anim_director.playableAsset.outputs.ToList();
            TrackAsset target_anim_track = playable_list[0].sourceObject as AnimationTrack;
            List<TimelineClip> track_clips = target_anim_track.GetClips().ToList();
            AnimationPlayableAsset target_anim_clip = track_clips[0].asset as AnimationPlayableAsset;
            target_anim_clip.position = start_pos;

            // Set control clip to be on fabula timeline
            controlAnim.sourceGameObject.exposedName = UnityEditor.GUID.Generate().ToString();
            director.SetReferenceValue(controlAnim.sourceGameObject.exposedName, animTimeline);


        }

        director.Play(timeline);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
